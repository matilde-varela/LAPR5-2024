export interface Patient {
    firstName: string;
    lastName: string;
    phone: string;
    email: string;
    address: string;
    emergencyContact: string;
    gender:string;
    datebirth: string;
    medicalConditions: string[];
    allergies: string[];
    description: string;
}