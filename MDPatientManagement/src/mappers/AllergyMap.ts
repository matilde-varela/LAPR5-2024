import { Allergy } from "../domain/allergy";
import { IAllergyDTO } from "../dto/IAllergyDTO";

export class AllergyMap {


  static toDTO(allergyResult: Allergy) : IAllergyDTO{
    return allergyResult.toDTO();
  }

  public static toPersistence(allergy: Allergy): any {
    return {
      code: allergy.code.toString(),
      designation: allergy.designation.value,
      description: allergy.description ? allergy.description.value : '',
      domainId: allergy.code.toString(),
    };
  }

  public static toDomain(raw: any): Allergy {
    const allergyDTO = {
      code: raw.code,
      designation: raw.designation,
      description: raw.description,
    };

    return Allergy.create(allergyDTO).getValue();
  }
}
