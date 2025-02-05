"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MedicalCondition = void 0;
const AggregateRoot_1 = require("../core/domain/AggregateRoot");
const Result_1 = require("../core/logic/Result");
const medicalConditionId_1 = require("./medicalConditionId");
class MedicalCondition extends AggregateRoot_1.AggregateRoot {
    get medicalConditionId() {
        return this.id;
    }
    get designation() {
        return this.props.designation;
    }
    set designation(value) {
        this.props.designation = value;
    }
    get description() {
        return this.props.description;
    }
    set description(value) {
        this.props.description = value;
    }
    get symptoms() {
        return this.props.symptoms;
    }
    set symptoms(value) {
        this.props.symptoms = value;
    }
    constructor(props, id) {
        super(props, id);
    }
    static create(medicalConditionDTO, p) {
        const idDto = medicalConditionDTO.id;
        const designationDto = medicalConditionDTO.designation;
        const descriptionDto = medicalConditionDTO.description;
        const symptomsDto = medicalConditionDTO.symptoms;
        if (!!designationDto === false || designationDto.length === 0) {
            return Result_1.Result.fail('Must provide a designation.');
        }
        else if (designationDto.length > 100) {
            return Result_1.Result.fail('Must provide a smaller designation.');
        }
        else if (!!idDto === false || idDto.length === 0) {
            return Result_1.Result.fail('Must provide an id.');
        }
        else if (!!descriptionDto === false || descriptionDto.length === 0) {
            return Result_1.Result.fail('Must provide a description.');
        }
        else if (descriptionDto.length > 2048) {
            return Result_1.Result.fail('Must provide a smaller description.');
        }
        else if (!!symptomsDto === false || symptomsDto.length === 0) {
            return Result_1.Result.fail('Must provide at least one symptom.');
        }
        else {
            const medicalCondition = new MedicalCondition({ designation: designationDto, description: descriptionDto, symptoms: symptomsDto }, new medicalConditionId_1.MedicalConditionId(idDto));
            return Result_1.Result.ok(medicalCondition);
        }
    }
}
exports.MedicalCondition = MedicalCondition;
//# sourceMappingURL=medicalCondition.js.map