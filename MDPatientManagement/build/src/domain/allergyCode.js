"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AllergyCode = void 0;
const UniqueEntityID_1 = require("../core/domain/UniqueEntityID");
const Guard_1 = require("../core/logic/Guard");
const Result_1 = require("../core/logic/Result");
const TextUtil_1 = require("../utils/TextUtil");
class AllergyCode extends UniqueEntityID_1.UniqueEntityID {
    get code() {
        return this.code;
    }
    constructor(code) {
        super(code);
    }
    static create(code) {
        const guardResult = Guard_1.Guard.againstNullOrUndefined(code, 'code');
        if (!TextUtil_1.TextUtil.isICD11Code(code)) {
            return Result_1.Result.fail("The provided code does not respect the ICD-11 format.");
        }
        if (!guardResult.succeeded) {
            return Result_1.Result.fail(guardResult.message);
        }
        else {
            return Result_1.Result.ok(new AllergyCode(code));
        }
    }
}
exports.AllergyCode = AllergyCode;
//# sourceMappingURL=allergyCode.js.map