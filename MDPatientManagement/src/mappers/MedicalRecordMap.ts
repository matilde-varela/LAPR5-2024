import { Document, Model } from "mongoose";
import { Mapper } from "../core/infra/Mapper";
import { IMedicalRecordPersistence } from "../dataschema/IMedicalRecordPersistence";
import { MedicalRecord } from "../domain/medicalRecord";
import { IMedicalRecordDTO } from "../dto/IMedicalRecordDTO";

export class MedicalRecordMap extends Mapper<MedicalRecord> {

    public static toDTO(medicalRecord: MedicalRecord): IMedicalRecordDTO {

        const id = medicalRecord.id.toString();
        const medicalRecordNumber = medicalRecord.props.medicalRecordNumber.value;
        const medicalConditions = medicalRecord.props.medicalConditions.map(condition => {
            const conditionValue = condition.toValue().toString();
            return conditionValue;
        });
        const allergies = medicalRecord.props.allergies.map(allergy => {
            const allergyValue = allergy.toValue().toString();
            return allergyValue;
        });
        const description = medicalRecord.props.description;
    
        const dto: IMedicalRecordDTO = {
            id,
            medicalRecordNumber,
            medicalConditions,
            allergies,
            description,
        };
    
        return dto;
    }
    

    public static toPersistence(medicalRecord: MedicalRecord): any {
        return { 
            domainId: medicalRecord.id.toString(),
            id: medicalRecord.id.toString(),
            medicalRecordNumber: medicalRecord.props.medicalRecordNumber.value,
            medicalConditions: medicalRecord.props.medicalConditions,
            allergies: medicalRecord.props.allergies,
            description: medicalRecord.props.description,
        };
    }
    
    

    public static toDomain(medicalRecord: any | Model<IMedicalRecordPersistence & Document>): MedicalRecord {
        const domainMedicalRecord = MedicalRecord.create({
            id: medicalRecord.id,
            medicalRecordNumber: medicalRecord.medicalRecordNumber,
            medicalConditions: medicalRecord.medicalConditions.map(item => item.value),
            allergies: medicalRecord.allergies.map(item => item.value),
            description: medicalRecord.description,
        });
    
        if (domainMedicalRecord.isFailure) {
            console.log("Error creating domain object:", domainMedicalRecord.error);
            return null;
        }
    
        return domainMedicalRecord.getValue();
    }
    
}