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
const MedicalRecordMap_1 = require("../mappers/MedicalRecordMap");
const medicalRecordId_1 = require("../domain/medicalRecordId");
let MedicalRecordRepo = class MedicalRecordRepo {
    constructor(medicalRecordSchema, logger) {
        this.medicalRecordSchema = medicalRecordSchema;
        this.logger = logger;
    }
    async save(medicalRecord) {
        const query = { domainId: medicalRecord.id.toString() };
        const medicalRecordDocument = await this.medicalRecordSchema.findOne(query);
        try {
            if (medicalRecordDocument != null) {
                throw new Error(`Medical Record with ID ${medicalRecord.medicalRecordId} already exists.`);
            }
            const rawMedicalRecord = MedicalRecordMap_1.MedicalRecordMap.toPersistence(medicalRecord);
            const medicalRecordCreated = await this.medicalRecordSchema.create(rawMedicalRecord);
            return MedicalRecordMap_1.MedicalRecordMap.toDomain(medicalRecordCreated);
        }
        catch (err) {
            throw err;
        }
    }
    async exists(medicalRecord) {
        const idX = medicalRecord.id instanceof medicalRecordId_1.MedicalRecordId ? medicalRecord.id.toValue() : medicalRecord.id;
        const query = { domainId: idX };
        const medicalRecordDocument = await this.medicalRecordSchema.findOne(query);
        return !!medicalRecordDocument === true;
    }
    async findByDomainId(medicalRecordId) {
        const query = { domainId: medicalRecordId };
        const medicalRecordRecord = await this.medicalRecordSchema.findOne(query);
        if (medicalRecordRecord != null) {
            return MedicalRecordMap_1.MedicalRecordMap.toDomain(medicalRecordRecord);
        }
        else
            return null;
    }
    async findAll() {
        try {
            const medicalRecordRecords = await this.medicalRecordSchema.find({});
            const medicalRecordList = await Promise.all(medicalRecordRecords.map(async (record) => await MedicalRecordMap_1.MedicalRecordMap.toDomain(record)));
            return medicalRecordList;
        }
        catch (error) {
            this.logger.error("Error fetching all medical record:", error);
        }
    }
    async findAllByParameters(filters) {
        const medicalRecordsList = [];
        // Use Promise.all for parallel queries
        const queryPromises = filters.queryfilters.map(filter => {
            var _a, _b;
            const query = {};
            if (((_a = filter.allergyDesignation) === null || _a === void 0 ? void 0 : _a.length) > 0) {
                query.allergyDesignation = filter.allergyDesignation;
            }
            if (((_b = filter.medicalConditionDesignation) === null || _b === void 0 ? void 0 : _b.length) > 0) {
                query.medicalConditionDesignation = filter.medicalConditionDesignation;
            }
            console.log('QUERY', query);
            // Return the query promise
            return this.medicalRecordSchema.find(query).exec();
        });
        // Wait for all promises to resolve
        const results = await Promise.all(queryPromises);
        // Flatten the results into one array
        results.forEach(records => {
            medicalRecordsList.push(...records);
        });
        // Map the records to their domain representation
        return medicalRecordsList.length > 0
            ? medicalRecordsList.map(MedicalRecordMap_1.MedicalRecordMap.toDomain)
            : [];
    }
};
MedicalRecordRepo = __decorate([
    (0, typedi_1.Service)(),
    __param(0, (0, typedi_1.Inject)('medicalRecordSchema')),
    __param(1, (0, typedi_1.Inject)('logger')),
    __metadata("design:paramtypes", [mongoose_1.Model, Object])
], MedicalRecordRepo);
exports.default = MedicalRecordRepo;
//# sourceMappingURL=medicalRecordRepo.js.map