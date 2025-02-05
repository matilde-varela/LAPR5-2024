export interface OperationRequest {
    id?: string
    deadLineDate: string;
    priority: string;
    dateOfRequest: string;
    status: string;
    staffId: string;
    description: string;
    patientId:string;
    operationTypeId: string;
}