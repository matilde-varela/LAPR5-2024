import { Repo } from "../../core/infra/Repo";
import { Allergy } from "../../domain/allergy";
import { AllergyCode } from "../../domain/allergyCode";
import { AllergyDesignation } from "../../domain/allergyDesignation";
import { IAllergyQueryFilterParametersDTO } from "../../dto/IAllergyQueryFilterParametersDTO";

export default interface IAllergyRepo extends Repo<Allergy> {
	save(allergy: Allergy): Promise<Allergy>;
	findByCode(code : string | AllergyCode) : Promise<Allergy>;
	findByDesignation(designation: string | AllergyDesignation): Promise<Allergy> 
	findAll() : Promise<Allergy[]>;
	findAllByParameters(allergyQueryParameters: IAllergyQueryFilterParametersDTO) : Promise<Allergy[]>
}
  