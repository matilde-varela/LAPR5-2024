import { Repo } from "../../core/infra/Repo";
import { MedicalRecord } from "../../domain/medicalRecord";
import { MedicalRecordId } from "../../domain/medicalRecordId";
import { IMedicalRecordQueryFilterParameters } from "../../dto/IMedicalRecordQueryFilterParameters";
import { IMedicalRecordQueryFilterParametersById } from "../../dto/IMedicalRecordQueryFiltersIds";

export default interface IMedicalRecordRepo extends Repo<MedicalRecord> {
    findAllByParameters(filters: IMedicalRecordQueryFilterParametersById): Promise<MedicalRecord[]>;
    save(medicalRecord: MedicalRecord): Promise<MedicalRecord>;
    findByDomainId (medicalRecordId: MedicalRecordId | string): Promise<MedicalRecord>;
    findAll() : Promise<MedicalRecord[]>
}