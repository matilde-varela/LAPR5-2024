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
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const typedi_1 = require("typedi");
const config_1 = __importDefault(require("../../config"));
const Result_1 = require("../core/logic/Result");
const medicalRecord_1 = require("../domain/medicalRecord");
const MedicalRecordMap_1 = require("../mappers/MedicalRecordMap");
const medicalConditionId_1 = require("../domain/medicalConditionId");
const allergyCode_1 = require("../domain/allergyCode");
let MedicalRecordService = class MedicalRecordService {
    constructor(medicalRecordRepo, logger) {
        this.medicalRecordRepo = medicalRecordRepo;
        this.logger = logger;
    }
    async createMedicalRecord(medicalRecordDTO) {
        try {
            const medicalRecordOrError = await medicalRecord_1.MedicalRecord.create(medicalRecordDTO);
            if (medicalRecordOrError.isFailure) {
                return Result_1.Result.fail(medicalRecordOrError.errorValue());
            }
            const medicalRecordResult = medicalRecordOrError.getValue();
            await this.medicalRecordRepo.save(medicalRecordResult);
            const medicalRecordDTOResult = MedicalRecordMap_1.MedicalRecordMap.toDTO(medicalRecordResult);
            return Result_1.Result.ok(medicalRecordDTOResult);
        }
        catch (e) {
            throw e;
        }
    }
    async updateMedicalRecord(medicalRecordDTO) {
        try {
            const medicalRecord = await this.medicalRecordRepo.findByDomainId(medicalRecordDTO.id);
            if (medicalRecord === null) {
                return Result_1.Result.fail("Patient Medical Record not found!");
            }
            else {
                const medicalConditionObjects = (medicalRecordDTO.medicalConditions || []).map((condition) => new medicalConditionId_1.MedicalConditionId(condition.toString()));
                const allergyObjects = (medicalRecordDTO.allergies || []).map((allergy) => new allergyCode_1.AllergyCode(allergy.toString()));
                medicalRecord.changeMedicalConditions(medicalConditionObjects);
                medicalRecord.changeAllergies(allergyObjects);
                medicalRecord.changeDescription(medicalRecordDTO.description);
                await this.medicalRecordRepo.save(medicalRecord);
                const medicalRecordDTOResult = MedicalRecordMap_1.MedicalRecordMap.toDTO(medicalRecord);
                return Result_1.Result.ok(medicalRecordDTOResult);
            }
        }
        catch (e) {
            throw e;
        }
    }
    async getAllMedicalRecords() {
        try {
            const records = await this.medicalRecordRepo.findAll();
            if (records === null || records.length == 0) {
                return Result_1.Result.fail("Medical Records not found");
            }
            else {
                const dto = MedicalRecordMap_1.MedicalRecordMap.toDTO(records[0]);
                console.log(dto);
                const recordsListDTOResult = records.map((record) => MedicalRecordMap_1.MedicalRecordMap.toDTO(record));
                // console.log(recordsListDTOResult[0].allergies)
                return Result_1.Result.ok(recordsListDTOResult);
            }
        }
        catch (e) {
            throw e;
        }
    }
    async getMedicalRecordsByFilters(filters) {
        try {
            const records = await this.medicalRecordRepo.findAllByParameters(filters);
            if (records.length == 0) {
                return Result_1.Result.fail("Medical records not found");
            }
            let medicalRecordsDtoList = [];
            for (var i = 0; i < records.length; i++) {
                const allergyDTO = MedicalRecordMap_1.MedicalRecordMap.toDTO(records.at(i));
                medicalRecordsDtoList.push(allergyDTO);
            }
            return Result_1.Result.ok(medicalRecordsDtoList);
        }
        catch (error) {
            throw new Error(`Failed to fetch medical records: ${error.message}`);
        }
    }
};
MedicalRecordService = __decorate([
    (0, typedi_1.Service)(),
    __param(0, (0, typedi_1.Inject)(config_1.default.repos.medicalRecord.name)),
    __param(1, (0, typedi_1.Inject)('logger')),
    __metadata("design:paramtypes", [Object, Object])
], MedicalRecordService);
exports.default = MedicalRecordService;
//# sourceMappingURL=medicalRecordService.js.map