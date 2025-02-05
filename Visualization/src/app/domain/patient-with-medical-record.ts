import { Allergy } from "./Allergy";
import { MedicalCondition } from "./MedicalCondition";

export interface PatientWithMedicalRecord {
    firstName: string;
    lastName: string;
    phone: string;
    email: string;
    address: string;
    emergencyContact: string;
    gender:string;
    dateBirth: string;
    medicalConditions: MedicalCondition[];
    allergies: Allergy[];
    description: string;
}