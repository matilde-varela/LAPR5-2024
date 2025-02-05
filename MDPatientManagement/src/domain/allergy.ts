import { AggregateRoot } from "../core/domain/AggregateRoot";
import { Result } from "../core/logic/Result";
import { AllergyCode } from "./allergyCode";
import { AllergyDesignation } from "./allergyDesignation";
import { AllergyDescription } from "./allergyDescription";
import { IAllergyDTO } from "../dto/IAllergyDTO";


interface AllergyProps {
  code : AllergyCode;
  designation : AllergyDesignation;
  description? : AllergyDescription; 
}

export class Allergy extends AggregateRoot<AllergyProps> {

  get code (): AllergyCode {
    return this.props.code;
  }

  get designation (): AllergyDesignation {
    return this.props.designation;
  }

  get description (): AllergyDescription {
    return this.props.description;
  }

  private constructor (props: AllergyProps) {
    super(props, props.code);
  }

  public static create(allergyDTO: IAllergyDTO): Result<Allergy> {
    const codeResult = AllergyCode.create(allergyDTO.code);
    if (codeResult.isFailure) {
        return Result.fail<Allergy>('Invalid allergy code');
    }
    const code = codeResult.getValue();

    const designationResult = AllergyDesignation.create(allergyDTO.designation);
    if (designationResult.isFailure) {
        return Result.fail<Allergy>('Invalid allergy designation');
    }
    const designation = designationResult.getValue();

    let description: AllergyDescription = null;
    if (allergyDTO.description) {
        const descriptionResult = AllergyDescription.create(allergyDTO.description);
        if (descriptionResult.isFailure) {
            return Result.fail<Allergy>('Invalid allergy description');
        }
        description = descriptionResult.getValue();
    }

    const allergy = new Allergy({ code, designation, description });
    return Result.ok<Allergy>(allergy);
}


  public changeDescription(description : string) {
    const descriptionResult = AllergyDescription.create(description);
    if (descriptionResult.isFailure) {
      throw new Error('Invalid description');
    }
    this.props.description = descriptionResult.getValue();
  }

  changeDesignation(designation: string) {
    const designationResult = AllergyDesignation.create(designation);
    if (designationResult.isFailure) {
      throw new Error('Invalid designation');
    }
    this.props.designation = designationResult.getValue();  }

  public toDTO(): IAllergyDTO {
    return {
      code: this.props.code.toString(),
      designation: this.props.designation.value,
      description: this.description ? this.description.value : null
    };
  }

}