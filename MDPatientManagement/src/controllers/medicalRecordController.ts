import { Inject, Service } from "typedi";
import IMedicalRecordService from "../services/IServices/IMedicalRecordService";
import config from "../../config";
import { NextFunction, Request, Response } from "express";
import { IMedicalRecordDTO } from "../dto/IMedicalRecordDTO";
import { Result } from "../core/logic/Result";
import IMedicalRecordController from "./IControllers/IMedicalRecordController";
import { IMedicalRecordQueryFilterParameters } from "../dto/IMedicalRecordQueryFilterParameters";
import { IExportMedicalRecordDTO } from "../dto/IExportMedicalRecordDTO";

@Service()
export default class MedicalRecordController implements IMedicalRecordController {

    constructor(
        @Inject(config.services.medicalRecord.name) private medicalRecordServiceInstance: IMedicalRecordService
    ) { }

    public async createMedicalRecord(req: Request, res: Response, next: NextFunction) {
        try {
            const medicalRecordOrError = await this.medicalRecordServiceInstance.createMedicalRecord(req.body as IMedicalRecordDTO) as Result<IMedicalRecordDTO>;

            if (medicalRecordOrError.isFailure) {
                return res.status(402).send();
            }

            const medicalRecordDTO = medicalRecordOrError.getValue();
            return res.status(201).json(medicalRecordDTO);
        } catch (e) {
            return next(e);
        }
    }

    public async updateMedicalRecord(req: Request, res: Response, next: NextFunction) {
        try {
            const medicalRecordOrError = await this.medicalRecordServiceInstance.updateMedicalRecord(req.body as IMedicalRecordDTO) as Result<IMedicalRecordDTO>;

            if (medicalRecordOrError.isFailure) {
                return res.status(404).send();
            }

            const medicalRecordDTO = medicalRecordOrError.getValue();
            return res.status(201).json(medicalRecordDTO);
        }
        catch (e) {
            return next(e);
        }
    }

    public async getAllMedicalRecords(req: Request, res: Response, next: NextFunction) {
        try {
            const medicalRecordListOrError = await this.medicalRecordServiceInstance.getAllMedicalRecords() as Result<IMedicalRecordDTO[]>;

            if(medicalRecordListOrError.isFailure){
                return res.status(402).send();
            }

            const medicalRecordListDTO = medicalRecordListOrError.getValue();
            return res.json(medicalRecordListDTO).status(200);
        } catch (e) {
            return next(e);
        }
    }

    public async getFilteredMedicalRecords(req: Request, res: Response, next: NextFunction) {
        try {
            const medicalRecordOrError = await this.medicalRecordServiceInstance.getMedicalRecordsByFilters(req.body as IMedicalRecordQueryFilterParameters) as Result<IMedicalRecordDTO[]>;
    
            if (medicalRecordOrError.isFailure) {
            return res.status(404).send();
            }
    
            const medicalRecordDTO = medicalRecordOrError.getValue();
            return res.status(201).json( medicalRecordDTO );
        }
        catch (e) {
            return next(e);
        }
    }

    public async exportMedicalRecords(req: Request, res: Response, next: NextFunction) {
        try {
            const successMessage = await this.medicalRecordServiceInstance.exportMedicalRecord(req.body as IExportMedicalRecordDTO) as Result<string>;
    
            if (successMessage.isFailure) {
            return res.status(404).send();
            }

            const filePath = successMessage.getValue();
            return res.status(201).json( filePath );
        }
        catch (e) {
            return next(e);
        }
    }

}