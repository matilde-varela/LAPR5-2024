"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
Object.defineProperty(exports, "__esModule", { value: true });
const typedi_1 = require("typedi");
const mongoose_1 = require("mongoose");
const allergyCode_1 = require("../domain/allergyCode");
const AllergyMap_1 = require("../mappers/AllergyMap");
let AllergyRepo = class AllergyRepo {
    constructor(allergyschema, logger) {
        this.allergyschema = allergyschema;
        this.logger = logger;
    }
    async findAllByParameters(allergyQueryParameters) {
        const allergyRecordsList = [];
        // Use Promise.all for parallel queries
        const queryPromises = allergyQueryParameters.queryfilters.map(filter => {
            var _a, _b, _c;
            const query = {};
            if (((_a = filter.code) === null || _a === void 0 ? void 0 : _a.length) > 0) {
                query.code = filter.code;
            }
            if (((_b = filter.designation) === null || _b === void 0 ? void 0 : _b.length) > 0) {
                query.designation = filter.designation;
            }
            if (((_c = filter.description) === null || _c === void 0 ? void 0 : _c.length) > 0) {
                query.description = filter.description;
            }
            // Return the query promise
            return this.allergyschema.find(query).exec();
        });
        // Wait for all promises to resolve
        const results = await Promise.all(queryPromises);
        // Flatten the results into one array
        results.forEach(records => {
            allergyRecordsList.push(...records);
        });
        // Map the records to their domain representation
        return allergyRecordsList.length > 0
            ? allergyRecordsList.map(AllergyMap_1.AllergyMap.toDomain)
            : [];
    }
    async exists(allergy) {
        const idX = allergy.code instanceof allergyCode_1.AllergyCode ? allergy.code.toValue() : allergy.code;
        const query = { code: idX };
        const allergyDocument = await this.allergyschema.findOne(query);
        return !!allergyDocument === true;
    }
    createBaseQuery() {
        return {
            where: {},
        };
    }
    async save(allergy) {
        const query = { code: allergy.code.toString() };
        const allergyDocument = await this.allergyschema.findOne(query);
        try {
            if (allergyDocument === null) {
                const rawAllergy = AllergyMap_1.AllergyMap.toPersistence(allergy);
                const allergyCreated = await this.allergyschema.create(rawAllergy);
                const allergyDomain = AllergyMap_1.AllergyMap.toDomain(allergyCreated);
                return allergyDomain;
            }
            else {
                console.log("Updating existing allergy");
                allergyDocument.designation = allergy.designation.value;
                allergyDocument.description = allergy.description ? allergy.description.value : '';
                await allergyDocument.save();
                return allergy;
            }
        }
        catch (err) {
            console.error('Error saving allergy:', err);
            throw err;
        }
    }
    async findByCode(code) {
        const idX = code instanceof allergyCode_1.AllergyCode ? code.toValue() : code;
        const query = { code: idX };
        const allergyRecord = await this.allergyschema.findOne(query);
        if (allergyRecord != null) {
            return AllergyMap_1.AllergyMap.toDomain(allergyRecord);
        }
        else
            return null;
    }
    async findAll() {
        try {
            const allergyRecords = await this.allergyschema.find({});
            const allergyList = await Promise.all(allergyRecords.map(async (record) => await AllergyMap_1.AllergyMap.toDomain(record)));
            return allergyList;
        }
        catch (error) {
            this.logger.error("Error fetching all allergies:", error);
        }
    }
};
AllergyRepo = __decorate([
    (0, typedi_1.Service)(),
    __param(0, (0, typedi_1.Inject)('allergySchema')),
    __param(1, (0, typedi_1.Inject)('logger')),
    __metadata("design:paramtypes", [mongoose_1.Model, Object])
], AllergyRepo);
exports.default = AllergyRepo;
//# sourceMappingURL=allergyRepo.js.map