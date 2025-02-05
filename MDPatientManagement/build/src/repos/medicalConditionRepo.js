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
const medicalConditionId_1 = require("../domain/medicalConditionId");
const MedicalConditionMap_1 = require("../mappers/MedicalConditionMap");
let MedicalConditionRepo = class MedicalConditionRepo {
    constructor(medicalConditionSchema, logger) {
        this.medicalConditionSchema = medicalConditionSchema;
        this.logger = logger;
    }
    createBaseQuery() {
        return {
            where: {},
        };
    }
    async exists(medicalCondition) {
        const idX = medicalCondition.id instanceof medicalConditionId_1.MedicalConditionId ? medicalCondition.id.toValue() : medicalCondition.id;
        const query = { domainId: idX };
        const medicalConditionDocument = await this.medicalConditionSchema.findOne(query);
        return !!medicalConditionDocument === true;
    }
    async save(medicalCondition) {
        const query = { domainId: medicalCondition.medicalConditionId.toString() };
        const medicalConditionDocument = await this.medicalConditionSchema.findOne(query);
        try {
            if (medicalConditionDocument !== null) {
                throw new Error(`Medical condition with ID ${medicalCondition.medicalConditionId} already exists.`);
            }
            const rawMedicalCondition = MedicalConditionMap_1.MedicalConditionMap.toPersistence(medicalCondition);
            const medicalConditionCreated = await this.medicalConditionSchema.create(rawMedicalCondition);
            return MedicalConditionMap_1.MedicalConditionMap.toDomain(medicalConditionCreated);
        }
        catch (err) {
            throw err;
        }
    }
    async findAll() {
        try {
            const conditionRecords = await this.medicalConditionSchema.find({});
            const conditionsList = await Promise.all(conditionRecords.map(async (record) => await MedicalConditionMap_1.MedicalConditionMap.toDomain(record)));
            return conditionsList;
        }
        catch (error) {
            this.logger.error("Error fetching all allergies:", error);
        }
    }
    async findByDomainId(medicalConditionId) {
        const query = { domainId: medicalConditionId };
        const medicalConditionRecord = await this.medicalConditionSchema.findOne(query);
        if (medicalConditionRecord != null) {
            return MedicalConditionMap_1.MedicalConditionMap.toDomain(medicalConditionRecord);
        }
        else
            return null;
    }
};
MedicalConditionRepo = __decorate([
    (0, typedi_1.Service)(),
    __param(0, (0, typedi_1.Inject)('medicalConditionSchema')),
    __param(1, (0, typedi_1.Inject)('logger')),
    __metadata("design:paramtypes", [mongoose_1.Model, Object])
], MedicalConditionRepo);
exports.default = MedicalConditionRepo;
//# sourceMappingURL=medicalConditionRepo.js.map