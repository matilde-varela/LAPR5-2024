import { Result } from "../../core/logic/Result";
import IMedicalConditionDTO from "../../dto/IMedicalConditionDTO";

export default interface IMedicalConditionService  {
  getMedicalConditionById(id: string): Promise<Result<IMedicalConditionDTO>>;
  getAllMedicalConditions(): Promise<Result<IMedicalConditionDTO[]>>;
  createMedicalCondition(roleDTO: IMedicalConditionDTO): Promise<Result<IMedicalConditionDTO>>;
}
