import { Request, Response, NextFunction } from 'express';
import { Inject, Service } from 'typedi';
import config from "../../config";
import { Result } from "../core/logic/Result";
import IMedicalConditionDTO from '../dto/IMedicalConditionDTO';
import IMedicalConditionController from './IControllers/IMedicalConditionController';
import IMedicalConditionService from '../services/IServices/IMedicalConditionService';

@Service()
export default class MedicalConditionController implements IMedicalConditionController /* TODO: extends ../core/infra/BaseController */ {
  constructor(
      @Inject(config.services.medicalCondition.name) private medicalConditionServiceInstance : IMedicalConditionService
  ) {}

  
  public async createMedicalCondition(req: Request, res: Response, next: NextFunction) {
    try {
      const medicalConditionOrError = await this.medicalConditionServiceInstance.createMedicalCondition(req.body as IMedicalConditionDTO) as Result<IMedicalConditionDTO>;
    
      if (medicalConditionOrError.isFailure) {
        return res.status(402).send();
      }

      const medicalConditionDTO = medicalConditionOrError.getValue();
      return res.json( medicalConditionDTO ).status(201);
    }
    catch (e) {
      return next(e);
    }
  };

  async getAllMedicalCondition(req: Request, res: Response, next: NextFunction) {
    try {
          const conditionOrError = await this.medicalConditionServiceInstance.getAllMedicalConditions() as Result<IMedicalConditionDTO[]>;
    
          if (conditionOrError.isFailure) {
            return res.status(404).send();
          }
    
          const conditionDTO = conditionOrError.getValue();
          return res.status(200).json( conditionDTO );
        }
        catch (e) {
          return next(e);
        }
  }

  async getMedicalConditionById(req: Request, res: Response, next: NextFunction) {
    try {

          const { id } = req.body;
      
          if (!id || typeof id !== 'string') {
            return res.status(400).json({ error: 'Invalid id provided.' });
          }

          const conditionOrError = await this.medicalConditionServiceInstance.getMedicalConditionById(id) as Result<IMedicalConditionDTO>;
    
          if (conditionOrError.isFailure) {
            return res.status(404).send();
          }
    
          const conditionDTO = conditionOrError.getValue();
          return res.status(200).json( conditionDTO );
        }
        catch (e) {
          return next(e);
        }
  }
}