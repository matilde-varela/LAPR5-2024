"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AllergyMap = void 0;
const allergy_1 = require("../domain/allergy");
class AllergyMap {
    static toDTO(allergyResult) {
        return allergyResult.toDTO();
    }
    static toPersistence(allergy) {
        return {
            code: allergy.code.toString(),
            designation: allergy.designation.value,
            description: allergy.description ? allergy.description.value : '',
            domainId: allergy.code.toString(),
        };
    }
    static toDomain(raw) {
        const allergyDTO = {
            code: raw.code,
            designation: raw.designation,
            description: raw.description,
        };
        return allergy_1.Allergy.create(allergyDTO).getValue();
    }
}
exports.AllergyMap = AllergyMap;
//# sourceMappingURL=AllergyMap.js.map