import { Result } from "../../core/logic/Result";
import { IExportMedicalRecordDTO } from "../../dto/IExportMedicalRecordDTO";
import { IMedicalRecordDTO } from "../../dto/IMedicalRecordDTO";
import { IMedicalRecordQueryFilterParameters } from "../../dto/IMedicalRecordQueryFilterParameters";

export default interface IMedicalRecordService {
    createMedicalRecord(medicalRecordDTO: IMedicalRecordDTO): Promise<Result<IMedicalRecordDTO>>;
    updateMedicalRecord(medicalRecordDTO: IMedicalRecordDTO): Promise<Result<IMedicalRecordDTO>>;
    getMedicalRecordsByFilters(filters : IMedicalRecordQueryFilterParameters): Promise<Result<IMedicalRecordDTO[]>>;
    getAllMedicalRecords(): Promise<Result<IMedicalRecordDTO[]>>;
    exportMedicalRecord(exportInfo: IExportMedicalRecordDTO): Promise<Result<string>>;
}