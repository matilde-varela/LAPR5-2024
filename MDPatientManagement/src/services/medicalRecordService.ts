import { Service, Inject } from 'typedi';
import IMedicalRecordService from "./IServices/IMedicalRecordService";
import config from "../../config";
import { Result } from '../core/logic/Result';
import { IMedicalRecordDTO } from '../dto/IMedicalRecordDTO';
import { MedicalRecord } from '../domain/medicalRecord';
import { MedicalRecordMap } from '../mappers/MedicalRecordMap';
import IMedicalRecordRepo from './IRepos/IMedicalRecordRepo';
import { MedicalConditionId } from '../domain/medicalConditionId';
import { AllergyCode } from '../domain/allergyCode';
import { IMedicalRecordQueryFilterParameters } from '../dto/IMedicalRecordQueryFilterParameters';
import IMedicalConditionRepo from './IRepos/IMedicalConditionRepo';
import IAllergyRepo from './IRepos/IAllergyRepo';
import { IExportMedicalRecordDTO } from '../dto/IExportMedicalRecordDTO';
import { PDFDocument, PDFFont, rgb, StandardFonts } from 'pdf-lib';
import { MedicalRecordId } from '../domain/medicalRecordId';
import path from 'path';
import fs from 'fs-extra';
import Minizip from "minizip-asm.js";
import { IMedicalRecordQueryFilterParametersById } from '../dto/IMedicalRecordQueryFiltersIds';


@Service()
export default class MedicalRecordService implements IMedicalRecordService {
    constructor(
        @Inject(config.repos.medicalRecord.name) private medicalRecordRepo: IMedicalRecordRepo,
        @Inject(config.repos.medicalCondition.name) private medicalConditionRepo: IMedicalConditionRepo,
        @Inject(config.repos.allergy.name) private allergyRepo: IAllergyRepo,
    ) { }

    async createMedicalRecord(medicalRecordDTO: IMedicalRecordDTO): Promise<Result<IMedicalRecordDTO>> {
        try {

            const medicalRecord = await this.medicalRecordRepo.findByDomainId(medicalRecordDTO.id);

            if (medicalRecord != null) {
                throw new Error("Already exists a Patient Medical Record with that ID!");
            }

            const medicalRecordOrError = await MedicalRecord.create(medicalRecordDTO);
            if (medicalRecordOrError.isFailure) {
                return Result.fail<IMedicalRecordDTO>(medicalRecordOrError.errorValue());
            }

            const medicalRecordResult = medicalRecordOrError.getValue();

            await this.medicalRecordRepo.save(medicalRecordResult);

            const medicalRecordDTOResult = MedicalRecordMap.toDTO(medicalRecordResult) as IMedicalRecordDTO;
            return Result.ok<IMedicalRecordDTO>(medicalRecordDTOResult)
        } catch (e) {
            throw e;
        }
    }

    async updateMedicalRecord(medicalRecordDTO: IMedicalRecordDTO): Promise<Result<IMedicalRecordDTO>> {
        try {
            const medicalRecord = await this.medicalRecordRepo.findByDomainId(medicalRecordDTO.id);

            if (medicalRecord === null) {
                return Result.fail<IMedicalRecordDTO>("Patient Medical Record not found!");
            }

            else {
                const medicalConditionObjects = (medicalRecordDTO.medicalConditions || []).map(
                    (condition) => new MedicalConditionId(condition.toString())
                );

                const allergyObjects = (medicalRecordDTO.allergies || []).map(
                    (allergy) => new AllergyCode(allergy.toString())
                );

                medicalRecord.changeMedicalConditions(medicalConditionObjects);
                medicalRecord.changeAllergies(allergyObjects);
                medicalRecord.changeDescription(medicalRecordDTO.description);

                await this.medicalRecordRepo.save(medicalRecord);

                const medicalRecordDTOResult = MedicalRecordMap.toDTO(medicalRecord) as IMedicalRecordDTO;
                return Result.ok<IMedicalRecordDTO>(medicalRecordDTOResult);
            }
        } catch (e) {
            throw e;
        }
    }

    async getAllMedicalRecords(): Promise<Result<IMedicalRecordDTO[]>> {
        try {
            const records = await this.medicalRecordRepo.findAll();

            if (records === null || records.length == 0) {
                return Result.fail<IMedicalRecordDTO[]>("Medical Records not found");
            }
            else {

                const recordsListDTOResult = records.map((record) => MedicalRecordMap.toDTO(record) as IMedicalRecordDTO);
                return Result.ok<IMedicalRecordDTO[]>(recordsListDTOResult)
            }
        } catch (e) {
            throw e;
        }
    }

    async getMedicalRecordsByFilters(filters: IMedicalRecordQueryFilterParameters): Promise<Result<IMedicalRecordDTO[]>> {
        try {    
          const updatedFilters = await this.convertDesignationsToIds(filters);
      
          const records = await this.medicalRecordRepo.findAllByParameters(updatedFilters);

          if (records.length == 0) {
            return Result.fail<IMedicalRecordDTO[]>("Medical records not found");
          }
      
          let medicalRecordsDtoList: IMedicalRecordDTO[] = [];
      
          for (var i = 0; i < records.length; i++) {
            const allergyDTO = MedicalRecordMap.toDTO(records.at(i)) as IMedicalRecordDTO;
            medicalRecordsDtoList.push(allergyDTO);
          }
      
          return Result.ok<IMedicalRecordDTO[]>(medicalRecordsDtoList);
        } catch (error) {
          throw new Error(`Failed to fetch medical records: ${error.message}`);
        }
      }
      
      private async convertDesignationsToIds(
        filters: IMedicalRecordQueryFilterParameters
      ): Promise<IMedicalRecordQueryFilterParametersById> {
        const updatedFilters: IMedicalRecordQueryFilterParametersById = { filters: [] };
      
        for (const filter of filters.filters) {
          const updatedFilter: any = {};
      
          if (filter.allergyDesignation && filter.allergyDesignation.length > 0) {
            const allergy = await this.allergyRepo.findByDesignation(filter.allergyDesignation);
            if (allergy) {
              updatedFilter.allergyCode = allergy.code.toString();
            }
          }
      
          if (filter.medicalConditionDesignation && filter.medicalConditionDesignation.length > 0) {
            const condition = await this.medicalConditionRepo.findByDesignation(filter.medicalConditionDesignation);
            if (condition) {
              updatedFilter.medicalConditionId = condition.id.toString();
            }
          }
      
          updatedFilters.filters.push(updatedFilter);
        }
      
        return updatedFilters;
      }

      async exportMedicalRecord(exportInfo: IExportMedicalRecordDTO): Promise<Result<string>> {
        const medicalRecord = await this.medicalRecordRepo.findByDomainId(new MedicalRecordId(exportInfo.medicalRecordNumber));

        if (!medicalRecord) {
            throw new Error("Medical Record wasn't found with this Medical Record Number.");
        }

        const pdfDoc = await PDFDocument.create();
        const timesRomanFont = await pdfDoc.embedFont(StandardFonts.TimesRoman);
        const boldFont = await pdfDoc.embedFont(StandardFonts.TimesRomanBold);

        let page = pdfDoc.addPage([500, 700]);
        page.setFont(timesRomanFont);

        let yPosition = 650;
        const lineHeight = 20;
        const sectionSpacing = 30;

        const drawDivider = () => {
            yPosition -= lineHeight / 2;
            page.drawLine({
                start: { x: 50, y: yPosition },
                end: { x: 450, y: yPosition },
                thickness: 1,
                color: rgb(0.8, 0.8, 0.8),
            });
            yPosition -= lineHeight;
        };

        page.setFont(boldFont);
        page.drawText('Medical Record', { x: 50, y: yPosition, size: 24 });
        yPosition -= sectionSpacing;

        page.setFont(timesRomanFont);
        page.drawText(`Record Number: ${exportInfo.medicalRecordNumber}`, { x: 50, y: yPosition, size: 14 });
        yPosition -= sectionSpacing;

        page.setFont(boldFont);
        page.drawText('Medical Conditions:', { x: 50, y: yPosition, size: 16 });
        yPosition -= lineHeight;

        page.setFont(timesRomanFont);
        if (medicalRecord.medicalConditions.length === 0) {
            page.drawText(`- No medical conditions associated.`, { x: 70, y: yPosition, size: 12 });
        } else {
            for (const conditionId of medicalRecord.medicalConditions) {
                const medicalCondition = await this.medicalConditionRepo.findByDomainId(conditionId);
                page.drawText(`- ${medicalCondition.designation}`, { x: 70, y: yPosition, size: 12 });
                yPosition -= lineHeight;
                if (yPosition < 50) {
                    yPosition = 650;
                    page = pdfDoc.addPage([500, 700]);
                    page.setFont(timesRomanFont);
                }
            }
        }
        yPosition -= sectionSpacing;

        page.setFont(boldFont);
        page.drawText('Allergies:', { x: 50, y: yPosition, size: 16 });
        yPosition -= lineHeight;

        page.setFont(timesRomanFont);
        if (medicalRecord.allergies.length === 0) {
            page.drawText(`- No allergies associated.`, { x: 70, y: yPosition, size: 12 });
        } else {
            for (const allergyCode of medicalRecord.allergies) {
                const allergy = await this.allergyRepo.findByCode(allergyCode);
                page.drawText(`- ${allergy.designation.value}`, { x: 70, y: yPosition, size: 12 });
                yPosition -= lineHeight;
                if (yPosition < 50) {
                    yPosition = 650;
                    page = pdfDoc.addPage([500, 700]);
                    page.setFont(timesRomanFont);
                }
            }
        }
        yPosition -= sectionSpacing;

        page.setFont(boldFont);
        page.drawText('Additional Notes:', { x: 50, y: yPosition, size: 16 });
        yPosition -= lineHeight;

        page.setFont(timesRomanFont);
        const noteLines = this.wrapText(medicalRecord.description, 400, timesRomanFont, 12);
        for (const line of noteLines) {
            page.drawText(line, { x: 70, y: yPosition, size: 12 });
            yPosition -= lineHeight;
            if (yPosition < 50) {
                yPosition = 650;
                page = pdfDoc.addPage([500, 700]);
                page.setFont(timesRomanFont);
            }
        }

        const pdfBytes = await pdfDoc.save();
        const pdfPath = path.join(exportInfo.filepath, "Medical_Record.pdf");
        const compressedPath = path.join(exportInfo.filepath, "Medical_Record.zip");
        fs.writeFileSync(pdfPath, pdfBytes);

        const mz = new Minizip();
        const files = await fs.readdir(exportInfo.filepath);

        for(let file of files) {
            const filePath = `${exportInfo.filepath}/${file}`;
            const fileData = await fs.readFile(pdfPath);
            mz.append(file,fileData,{password: exportInfo.pass});
        }

        const data = new Uint8Array(mz.zip());
        await fs.writeFile(compressedPath,data);

        fs.unlinkSync(pdfPath);

        return Result.ok<string>(pdfPath);
    }

    private wrapText(text: string, maxWidth: number, font: PDFFont, fontSize: number): string[] {
        const words = text.split(' ');
        const lines: string[] = [];
        let currentLine = '';

        for (const word of words) {
            const testLine = currentLine ? `${currentLine} ${word}` : word;
            const testWidth = font.widthOfTextAtSize(testLine, fontSize);

            if (testWidth > maxWidth) {
                lines.push(currentLine);
                currentLine = word;
            } else {
                currentLine = testLine;
            }
        }

        if (currentLine) {
            lines.push(currentLine);
        }

        return lines;
    }

}
