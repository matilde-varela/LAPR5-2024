import { Request, Response, NextFunction } from 'express';

export default interface IMedicalConditionController  {
  createMedicalCondition(req: Request, res: Response, next: NextFunction);
  getAllMedicalCondition(req: Request, res: Response, next: NextFunction);
  getMedicalConditionById(req: Request, res: Response, next: NextFunction);
}