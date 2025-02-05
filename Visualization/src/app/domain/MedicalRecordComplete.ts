import { Allergy } from "./Allergy";
import { MedicalCondition } from "./MedicalCondition";

export interface MedicalRecordComplete {
    id: string;
    medicalRecordNumber: string;
    medicalConditions: MedicalCondition[];
    allergies: Allergy[];
    description: string;
}
