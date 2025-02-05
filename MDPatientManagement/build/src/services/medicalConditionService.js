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
const medicalCondition_1 = require("../domain/medicalCondition");
const MedicalConditionMap_1 = require("../mappers/MedicalConditionMap");
let MedicalConditionService = class MedicalConditionService {
    constructor(medicalConditionRepo) {
        this.medicalConditionRepo = medicalConditionRepo;
    }
    async createMedicalCondition(medicalConditionDTO) {
        try {
            const medicalConditionOrError = await medicalCondition_1.MedicalCondition.create(medicalConditionDTO);
            if (medicalConditionOrError.isFailure) {
                return Result_1.Result.fail(medicalConditionOrError.errorValue());
            }
            const medicalConditionResult = medicalConditionOrError.getValue();
            await this.medicalConditionRepo.save(medicalConditionResult);
            const medicalConditionDTOResult = MedicalConditionMap_1.MedicalConditionMap.toDTO(medicalConditionResult);
            return Result_1.Result.ok(medicalConditionDTOResult);
        }
        catch (e) {
            throw e;
        }
    }
    async getMedicalConditionById(id) {
        try {
            const condition = await this.medicalConditionRepo.findByDomainId(id);
            if (!condition) {
                return Result_1.Result.fail("Allergy not found");
            }
            const conditionDTO = MedicalConditionMap_1.MedicalConditionMap.toDTO(condition);
            return Result_1.Result.ok(conditionDTO);
        }
        catch (error) {
            throw new Error(`Failed to fetch allergy: ${error.message}`);
        }
    }
    async getAllMedicalConditions() {
        return new Promise(async (resolve, reject) => {
            try {
                const conditions = await this.medicalConditionRepo.findAll();
                if (conditions === null || conditions.length === 0) {
                    resolve(Result_1.Result.fail("Medical Conditions not found"));
                }
                else {
                    const conditionsListDTOResult = conditions.map((condition) => MedicalConditionMap_1.MedicalConditionMap.toDTO(condition));
                    resolve(Result_1.Result.ok(conditionsListDTOResult));
                }
            }
            catch (e) {
                reject(e);
            }
        });
    }
};
MedicalConditionService = __decorate([
    (0, typedi_1.Service)(),
    __param(0, (0, typedi_1.Inject)(config_1.default.repos.medicalCondition.name)),
    __metadata("design:paramtypes", [Object])
], MedicalConditionService);
exports.default = MedicalConditionService;
//# sourceMappingURL=medicalConditionService.js.map