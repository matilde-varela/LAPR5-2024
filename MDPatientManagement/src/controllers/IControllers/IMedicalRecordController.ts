import { Request, Response, NextFunction } from 'express';

export default interface IMedicalConditionController {
    createMedicalRecord(req: Request, res: Response, next: NextFunction);
    updateMedicalRecord(req: Request, res: Response, next: NextFunction);
    getFilteredMedicalRecords(req: Request, res: Response, next: NextFunction);
    getAllMedicalRecords(req: Request, res: Response, next: NextFunction);
    exportMedicalRecords(req: Request, res: Response, next: NextFunction);
}