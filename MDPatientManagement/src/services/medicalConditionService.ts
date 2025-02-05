import { Inject, Service } from "typedi";
import IMedicalRecordService from "./IServices/IMedicalConditionService";
import config from "../../config";
import { Result } from "../core/logic/Result";
import IMedicalConditionDTO from "../dto/IMedicalConditionDTO";
import { MedicalCondition } from "../domain/medicalCondition";
import { MedicalConditionId } from "../domain/medicalConditionId";
import IMedicalConditionRepo from "./IRepos/IMedicalConditionRepo";
import { MedicalConditionMap } from "../mappers/MedicalConditionMap";
import IMedicalConditionService from "./IServices/IMedicalConditionService";


@Service()
export default class MedicalConditionService implements IMedicalConditionService {
  constructor(
      @Inject(config.repos.medicalCondition.name) private medicalConditionRepo : IMedicalConditionRepo
  ) {}

  public async createMedicalCondition(medicalConditionDTO: IMedicalConditionDTO): Promise<Result<IMedicalConditionDTO>> {
    try {
        const medicalConditionOrError = await MedicalCondition.create(medicalConditionDTO );
  
        if (medicalConditionOrError.isFailure) {
          return Result.fail<IMedicalConditionDTO>(medicalConditionOrError.errorValue());
        }
  
        const medicalConditionResult = medicalConditionOrError.getValue();
  
        await this.medicalConditionRepo.save(medicalConditionResult);
  
        const medicalConditionDTOResult = MedicalConditionMap.toDTO( medicalConditionResult ) as IMedicalConditionDTO;
        return Result.ok<IMedicalConditionDTO>( medicalConditionDTOResult )
      } catch (e) {
        throw e;
      }
    }


  async getMedicalConditionById(id: string): Promise<Result<IMedicalConditionDTO>> {
    try {
          const condition = await this.medicalConditionRepo.findByDomainId(id);
          if (!condition) {
            return Result.fail<IMedicalConditionDTO>("Allergy not found");
          }
    
          const conditionDTO = MedicalConditionMap.toDTO(condition) as IMedicalConditionDTO;
          return Result.ok<IMedicalConditionDTO>(conditionDTO);
        } catch (error) {
          throw new Error(`Failed to fetch allergy: ${error.message}`);
        }
  }

  async getMedicalConditionByDesignation(designation: string): Promise<Result<IMedicalConditionDTO>> {
    try {
      const condition = await this.medicalConditionRepo.findByDesignation(designation);
      
      if (!condition) {
        return Result.fail<IMedicalConditionDTO>("Medical condition not found");
      }
  
      const conditionDTO = MedicalConditionMap.toDTO(condition) as IMedicalConditionDTO;
      
      return Result.ok<IMedicalConditionDTO>(conditionDTO);
    } catch (error) {
      throw new Error(`Failed to fetch medical condition: ${error.message}`);
    }
  }


  async getAllMedicalConditions(): Promise<Result<IMedicalConditionDTO[]>> {
    return new Promise(async (resolve, reject) => {
      try {
        const conditions = await this.medicalConditionRepo.findAll();
  
        if (conditions === null || conditions.length === 0) {
          resolve(Result.fail<IMedicalConditionDTO[]>("Medical Conditions not found"));
        } else {
          const conditionsListDTOResult = conditions.map((condition) => MedicalConditionMap.toDTO(condition) as IMedicalConditionDTO);
          resolve(Result.ok<IMedicalConditionDTO[]>(conditionsListDTOResult));
        }
      } catch (e) {
        reject(e);
      }
    });
  }
  
}
