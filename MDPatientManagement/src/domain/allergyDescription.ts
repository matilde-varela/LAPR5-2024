import { ValueObject } from "../core/domain/ValueObject";
import { Result } from "../core/logic/Result";
import { Guard } from "../core/logic/Guard";

interface AllergyDescriptionProps {
  value: string;
}

export class AllergyDescription extends ValueObject<AllergyDescriptionProps> {
  get value (): string {
    return this.props.value;
  }
  
  private constructor (props: AllergyDescriptionProps) {
    super(props);
  }

  public static create (description: string): Result<AllergyDescription> {
    const guardResult = Guard.againstNullOrUndefined(description, 'description');
    if (!guardResult.succeeded) {
      return Result.fail<AllergyDescription>(guardResult.message);
    } else {
      return Result.ok<AllergyDescription>(new AllergyDescription({ value: description }))
    }
  }
}