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
const AllergyMap_1 = require("../mappers/AllergyMap");
const allergy_1 = require("../domain/allergy");
const Result_1 = require("../core/logic/Result");
let AllergyService = class AllergyService {
    constructor(allergyRepo, logger) {
        this.allergyRepo = allergyRepo;
        this.logger = logger;
    }
    async getAllergiesByFilters(arg0) {
        try {
            const allergies = await this.allergyRepo.findAllByParameters(arg0);
            if (allergies.length == 0) {
                return Result_1.Result.fail("Allergy not found");
            }
            let allergyDtoList = [];
            for (var i = 0; i < allergies.length; i++) {
                const allergyDTO = AllergyMap_1.AllergyMap.toDTO(allergies.at(i));
                allergyDtoList.push(allergyDTO);
            }
            return Result_1.Result.ok(allergyDtoList);
        }
        catch (error) {
            throw new Error(`Failed to fetch allergy: ${error.message}`);
        }
    }
    async createAllergy(allergyDTO) {
        try {
            const allergyOrError = await allergy_1.Allergy.create(allergyDTO);
            if (allergyOrError.isFailure) {
                return Result_1.Result.fail(allergyOrError.errorValue());
            }
            const allergyResult = allergyOrError.getValue();
            await this.allergyRepo.save(allergyResult);
            const allergyDTOResult = AllergyMap_1.AllergyMap.toDTO(allergyResult);
            return Result_1.Result.ok(allergyDTOResult);
        }
        catch (e) {
            throw e;
        }
    }
    async updateAllergy(allergyDTO) {
        try {
            const allergy = await this.allergyRepo.findByCode(allergyDTO.code);
            if (allergy === null) {
                return Result_1.Result.fail("Allergy not found");
            }
            else {
                allergy.changeDesignation(allergyDTO.designation);
                allergy.changeDescription(allergyDTO.description);
                await this.allergyRepo.save(allergy);
                const allergyDTOResult = AllergyMap_1.AllergyMap.toDTO(allergy);
                return Result_1.Result.ok(allergyDTOResult);
            }
        }
        catch (e) {
            throw e;
        }
    }
    async getAllergyByCode(code) {
        try {
            const allergy = await this.allergyRepo.findByCode(code);
            if (!allergy) {
                return Result_1.Result.fail("Allergy not found");
            }
            const allergyDTO = AllergyMap_1.AllergyMap.toDTO(allergy);
            return Result_1.Result.ok(allergyDTO);
        }
        catch (error) {
            throw new Error(`Failed to fetch allergy: ${error.message}`);
        }
    }
    async getAllAllergies() {
        try {
            const allergies = await this.allergyRepo.findAll();
            if (allergies === null || allergies.length == 0) {
                return Result_1.Result.fail("Allergies not found");
            }
            else {
                const allergiesListDTOResult = allergies.map((allergy) => AllergyMap_1.AllergyMap.toDTO(allergy));
                return Result_1.Result.ok(allergiesListDTOResult);
            }
        }
        catch (e) {
            throw e;
        }
    }
};
AllergyService = __decorate([
    (0, typedi_1.Service)(),
    __param(0, (0, typedi_1.Inject)(config_1.default.repos.allergy.name)),
    __param(1, (0, typedi_1.Inject)('logger')),
    __metadata("design:paramtypes", [Object, Object])
], AllergyService);
exports.default = AllergyService;
//# sourceMappingURL=allergyService.js.map