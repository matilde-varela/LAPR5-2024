"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AllergyDesignation = void 0;
const ValueObject_1 = require("../core/domain/ValueObject");
const Result_1 = require("../core/logic/Result");
const Guard_1 = require("../core/logic/Guard");
class AllergyDesignation extends ValueObject_1.ValueObject {
    get value() {
        return this.props.value;
    }
    constructor(props) {
        super(props);
    }
    static create(designation) {
        const guardResult = Guard_1.Guard.againstNullOrUndefined(designation, 'designation');
        if (!guardResult.succeeded) {
            return Result_1.Result.fail(guardResult.message);
        }
        else {
            return Result_1.Result.ok(new AllergyDesignation({ value: designation }));
        }
    }
}
exports.AllergyDesignation = AllergyDesignation;
//# sourceMappingURL=allergyDesignation.js.map