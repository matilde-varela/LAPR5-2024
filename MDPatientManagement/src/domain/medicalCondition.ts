import { AggregateRoot } from "../core/domain/AggregateRoot";
import { UniqueEntityID } from "../core/domain/UniqueEntityID";
import { Result } from "../core/logic/Result";
import IMedicalConditionDTO from "../dto/IMedicalConditionDTO";
import { MedicalConditionId } from "./medicalConditionId";

interface MedicalConditionProps {
  designation: string;
  description: string;
  symptoms: string[];
}

export class MedicalCondition extends AggregateRoot<MedicalConditionProps> {

  get medicalConditionId(): MedicalConditionId {
    return this.id;
  }

  get designation(): string {
    return this.props.designation;
  }

  set designation(value: string) {
    this.props.designation = value;
  }

  get description(): string {
    return this.props.description;
  }

  set description(value: string) {
    this.props.description = value;
  }

  get symptoms(): string[] {
    return this.props.symptoms;
  }

  set symptoms(value: string[]) {
    this.props.symptoms = value;
  }

  public constructor(props: MedicalConditionProps, id: MedicalConditionId) {
    super(props, id);
  }

  public static create(medicalConditionDTO: IMedicalConditionDTO, p?: UniqueEntityID): Result<MedicalCondition> {
    const idDto = medicalConditionDTO.id;
    const designationDto = medicalConditionDTO.designation;
    const descriptionDto = medicalConditionDTO.description;
    const symptomsDto = medicalConditionDTO.symptoms;

    if (!!designationDto === false || designationDto.length === 0) {
      return Result.fail<MedicalCondition>('Must provide a designation.')
    } else if (designationDto.length > 100) {
      return Result.fail<MedicalCondition>('Must provide a smaller designation.')
    } else if (!!idDto === false || idDto.length === 0) {
      return Result.fail<MedicalCondition>('Must provide an id.')
    } else if (!!descriptionDto === false || descriptionDto.length === 0) {
      return Result.fail<MedicalCondition>('Must provide a description.')
    } else if (descriptionDto.length > 2048) {
      return Result.fail<MedicalCondition>('Must provide a smaller description.')
    } else if (!!symptomsDto === false || symptomsDto.length === 0) {
      return Result.fail<MedicalCondition>('Must provide at least one symptom.')
    } else {
      const medicalCondition = new MedicalCondition({ designation: designationDto, description: descriptionDto, symptoms: symptomsDto }, new MedicalConditionId(idDto));
      return Result.ok<MedicalCondition>(medicalCondition)
    }
  }
}
