"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MedicalConditionMap = void 0;
const Mapper_1 = require("../core/infra/Mapper");
const medicalCondition_1 = require("../domain/medicalCondition");
const UniqueEntityID_1 = require("../core/domain/UniqueEntityID");
class MedicalConditionMap extends Mapper_1.Mapper {
    static toDTO(medicalCondition) {
        return {
            id: medicalCondition.id.toString(),
            designation: medicalCondition.designation,
            description: medicalCondition.description,
            symptoms: medicalCondition.symptoms
        };
    }
    static toDomain(medicalCondition) {
        const medicalConditionOrError = medicalCondition_1.MedicalCondition.create(medicalCondition, new UniqueEntityID_1.UniqueEntityID(medicalCondition.domainId));
        medicalConditionOrError.isFailure ? console.log(medicalConditionOrError.error) : '';
        return medicalConditionOrError.isSuccess ? medicalConditionOrError.getValue() : null;
    }
    static toPersistence(medicalCondition) {
        return {
            domainId: medicalCondition.id.toString(),
            id: medicalCondition.id.toString(),
            designation: medicalCondition.designation,
            description: medicalCondition.description,
            symptoms: medicalCondition.symptoms
        };
    }
}
exports.MedicalConditionMap = MedicalConditionMap;
//# sourceMappingURL=MedicalConditionMap.js.map