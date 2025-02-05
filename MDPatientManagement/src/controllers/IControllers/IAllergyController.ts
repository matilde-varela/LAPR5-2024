import { NextFunction, Request, Response } from "express";

export default interface IAllergyController  {
  createAllergy(req: Request, res: Response, next: NextFunction);
  updateAllergy(req: Request, res: Response, next: NextFunction);
  getAllergyByCode(req: Request, res: Response, next: NextFunction);
  getAllAllergies(req: Request, res: Response, next: NextFunction);
  getAllergiesByFilter(req: Request, res: Response, next: NextFunction);
  
}