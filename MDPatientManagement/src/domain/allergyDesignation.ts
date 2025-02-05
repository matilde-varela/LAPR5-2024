import { ValueObject } from "../core/domain/ValueObject";
import { Result } from "../core/logic/Result";
import { Guard } from "../core/logic/Guard";

interface AllergyDesignationProps {
  value: string;
}

export class AllergyDesignation extends ValueObject<AllergyDesignationProps> {
  get value (): string {
    return this.props.value;
  }
  
  private constructor (props: AllergyDesignationProps) {
    super(props);
  }

  public static create (designation: string): Result<AllergyDesignation> {
    const guardResult = Guard.againstNullOrUndefined(designation, 'designation');
    if (!guardResult.succeeded) {
      return Result.fail<AllergyDesignation>(guardResult.message);
    } else {
      return Result.ok<AllergyDesignation>(new AllergyDesignation({ value: designation }))
    }
  }
}