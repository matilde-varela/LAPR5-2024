"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Allergy = void 0;
const AggregateRoot_1 = require("../core/domain/AggregateRoot");
const Result_1 = require("../core/logic/Result");
const allergyCode_1 = require("./allergyCode");
const allergyDesignation_1 = require("./allergyDesignation");
const allergyDescription_1 = require("./allergyDescription");
class Allergy extends AggregateRoot_1.AggregateRoot {
    get code() {
        return this.props.code;
    }
    get designation() {
        return this.props.designation;
    }
    get description() {
        return this.props.description;
    }
    constructor(props) {
        super(props, props.code);
    }
    static create(allergyDTO) {
        const codeResult = allergyCode_1.AllergyCode.create(allergyDTO.code);
        if (codeResult.isFailure) {
            return Result_1.Result.fail('Invalid allergy code');
        }
        const code = codeResult.getValue();
        const designationResult = allergyDesignation_1.AllergyDesignation.create(allergyDTO.designation);
        if (designationResult.isFailure) {
            return Result_1.Result.fail('Invalid allergy designation');
        }
        const designation = designationResult.getValue();
        let description = null;
        if (allergyDTO.description) {
            const descriptionResult = allergyDescription_1.AllergyDescription.create(allergyDTO.description);
            if (descriptionResult.isFailure) {
                return Result_1.Result.fail('Invalid allergy description');
            }
            description = descriptionResult.getValue();
        }
        const allergy = new Allergy({ code, designation, description });
        return Result_1.Result.ok(allergy);
    }
    changeDescription(description) {
        const descriptionResult = allergyDescription_1.AllergyDescription.create(description);
        if (descriptionResult.isFailure) {
            throw new Error('Invalid description');
        }
        this.props.description = descriptionResult.getValue();
    }
    changeDesignation(designation) {
        const designationResult = allergyDesignation_1.AllergyDesignation.create(designation);
        if (designationResult.isFailure) {
            throw new Error('Invalid designation');
        }
        this.props.designation = designationResult.getValue();
    }
    toDTO() {
        return {
            code: this.props.code.toString(),
            designation: this.props.designation.value,
            description: this.description ? this.description.value : null
        };
    }
}
exports.Allergy = Allergy;
//# sourceMappingURL=allergy.js.map