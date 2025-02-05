import { Repo } from "../../core/infra/Repo";
import { MedicalCondition } from "../../domain/medicalCondition";
import { MedicalConditionId } from "../../domain/medicalConditionId";

export default interface IMedicalCondtionRepo extends Repo<MedicalCondition> {
  save(medicalCondition: MedicalCondition): Promise<MedicalCondition>;
  findByDomainId (medicalConditionId: MedicalConditionId | string): Promise<MedicalCondition>;
  findAll() : Promise<MedicalCondition[]>;
  findByDesignation(designation: string): Promise<MedicalCondition>;
  
    
  //findByIds (rolesIds: RoleId[]): Promise<Role[]>;
  //saveCollection (roles: Role[]): Promise<Role[]>;
  //removeByRoleIds (roles: RoleId[]): Promise<any>
}