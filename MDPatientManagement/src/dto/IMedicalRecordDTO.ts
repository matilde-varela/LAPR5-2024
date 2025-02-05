export interface IMedicalRecordDTO {
    id: string;
    medicalRecordNumber: string;
    medicalConditions?: string[];
    allergies?: string[];
    description?: string;
}