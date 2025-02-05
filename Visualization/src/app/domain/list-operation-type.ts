import { Phase } from "./Phase";
import { RequiredStaff } from "./RequiredStaff";

export interface ListOperationType {
    name: string;
    estimatedDuration: number; 
    status: boolean;        
    requiredStaff: RequiredStaff[]; 
    phases: Phase[];  
}