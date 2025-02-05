import { Document, Model } from "mongoose";
import { Mapper } from "../core/infra/Mapper";
import { IMedicalConditionPersistence } from "../dataschema/IMedicalConditionPersistence";
import { MedicalCondition } from "../domain/medicalCondition";
import IMedicalConditionDTO from "../dto/IMedicalConditionDTO";
import { UniqueEntityID } from "../core/domain/UniqueEntityID";
import medicalConditionSchema from "../persistence/schemas/medicalConditionSchema";


export class MedicalConditionMap extends Mapper<MedicalCondition> {
  
  public static toDTO( medicalCondition: MedicalCondition): IMedicalConditionDTO {
    return {
      id: medicalCondition.id.toString(),
      designation: medicalCondition.designation,
      description: medicalCondition.description,
      symptoms: medicalCondition.symptoms
    } as IMedicalConditionDTO;
  }

  public static toDomain (medicalCondition: any | Model<IMedicalConditionPersistence & Document> ): MedicalCondition {
    const medicalConditionOrError = MedicalCondition.create(
      medicalCondition,
      new UniqueEntityID(medicalCondition.domainId)
    );

    medicalConditionOrError.isFailure ? console.log(medicalConditionOrError.error) : '';

    return medicalConditionOrError.isSuccess ? medicalConditionOrError.getValue() : null;
  }

  public static toPersistence (medicalCondition: MedicalCondition): any {
    return {
      domainId: medicalCondition.id.toString(),
      id: medicalCondition.id.toString(),
      designation: medicalCondition.designation,
      description: medicalCondition.description,
      symptoms: medicalCondition.symptoms
    }
  }
}