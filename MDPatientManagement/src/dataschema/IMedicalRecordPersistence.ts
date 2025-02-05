import { AllergyCode } from "../domain/allergyCode";
import { MedicalConditionId } from "../domain/medicalConditionId";
import { MedicalRecordNumber } from "../domain/medicalRecordNumber";

export interface IMedicalRecordPersistence {
    id: string;
    medicalRecordNumber: MedicalRecordNumber;
    medicalConditions: MedicalConditionId[];
    allergies: AllergyCode[];
    description: string;
}