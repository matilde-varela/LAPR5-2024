import "reflect-metadata";
import { expect } from "chai";
import { it } from 'mocha';
import sinon from "sinon";
import { Container } from "typedi";
import { IMedicalRecordDTO } from "../../../src/dto/IMedicalRecordDTO";
import IMedicalRecordRepo from "../../../src/services/IRepos/IMedicalRecordRepo";
import MedicalRecordService from "../../../src/services/medicalRecordService";
import { MedicalRecord } from "../../../src/domain/medicalRecord";
import { MedicalRecordMap } from "../../../src/mappers/MedicalRecordMap";
import IMedicalConditionRepo from "../../../src/services/IRepos/IMedicalConditionRepo";
import IAllergyRepo from "../../../src/services/IRepos/IAllergyRepo";
import { IMedicalRecordQueryFilterParameters } from "../../../src/dto/IMedicalRecordQueryFilterParameters";
import fs from 'fs-extra';
import Minizip from "minizip-asm.js";
import { IExportMedicalRecordDTO } from "../../../src/dto/IExportMedicalRecordDTO";
import * as tmp from 'tmp';


describe("MedicalRecordService Unit Tests", function () {
  const sandbox = sinon.createSandbox();
  this.timeout(5000);

  beforeEach(() => {
    Container.reset();

    const mockLogger = {
      info: sandbox.stub(),
      error: sandbox.stub(),
      warn: sandbox.stub(),
      debug: sandbox.stub(),
    };
    Container.set("logger", mockLogger);

    let medicalRecordSchemaInstance = require("../../../src/persistence/schemas/medicalRecordSchema").default;
    Container.set("medicalRecordSchema", medicalRecordSchemaInstance);

    let medicalRecordRepoClass = require("../../../src/repos/medicalRecordRepo").default;
    let medicalRecordRepoInstance = Container.get(medicalRecordRepoClass);
    Container.set("MedicalRecordRepo", medicalRecordRepoInstance);

    const medicalConditionRepoInstance = {
      findByDomainId: sandbox.stub().resolves({ designation: "Condition1" }),
      findByDesignation: sandbox.stub().resolves({ id: 'FA14.0', designation: "Asthma" }),
    };
    Container.set("MedicalConditionRepo", medicalConditionRepoInstance);

    const allergyRepoInstance = {
      findByCode: sandbox.stub().resolves({ designation: { value: "Allergy1" } }),
      findByDesignation: sandbox.stub().resolves({ code: 'FA33.0', designation: "Peanuts" }),
    };
    Container.set("AllergyRepo", allergyRepoInstance);
  });

  afterEach(() => {
    sandbox.restore();
    sinon.restore();
  });

  it("should create a new medical record successfully", async () => {
    const medicalRecordDTO: IMedicalRecordDTO = {
      id: "202501000001",
      medicalRecordNumber: "202501000001",
      medicalConditions: ["FA22.0"],
      allergies: ["BZ02.2"],
      description: "Patient has a mild condition.",
    };

    const medicalRecordRepoInstance = Container.get("MedicalRecordRepo") as IMedicalRecordRepo;
    const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
    const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;

    const medicalRecordMock = {
      id: "202501000001",
      medicalRecordNumber: "202501000001",
      medicalConditions: ["FA22.0"],
      allergies: ["BZ02.2"],
      description: "Patient has a mild condition.",
    };

    sinon.stub(medicalRecordRepoInstance, "findByDomainId").resolves(null);
    sinon.stub(medicalRecordRepoInstance, "save").resolves();
    sinon.stub(MedicalRecordMap, "toDTO").callsFake(() => medicalRecordMock);

    const service = new MedicalRecordService(medicalRecordRepoInstance, medicalConditionRepoInstance, allergyRepoInstance);
    const result = await service.createMedicalRecord(medicalRecordDTO);

    expect(result.isSuccess).to.be.true;
    expect(result.getValue().medicalRecordNumber).to.equal("202501000001");
  });


  it("should update an existing medical record successfully", async () => {
    const medicalRecordDTO: IMedicalRecordDTO = {
      id: "202501000001",
      medicalRecordNumber: "202501000001",
      medicalConditions: ['FA22.0'],
      allergies: ['BZ02.2'],
      description: "Patient has a mild condition.",
    };

    const medicalRecordDTO2: IMedicalRecordDTO = {
      id: "202501000001",
      medicalRecordNumber: "202501000001",
      medicalConditions: ['FA33.0'],
      allergies: ['BZ02.2'],
      description: "Patient has a mild condition.",
    };


    const medicalRecordRepoInstance = Container.get("MedicalRecordRepo") as IMedicalRecordRepo;
    const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
    const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;

    const existingRecord = MedicalRecord.create(medicalRecordDTO).getValue();
    sinon.stub(medicalRecordRepoInstance, "findByDomainId").resolves(existingRecord);
    sinon.stub(medicalRecordRepoInstance, "save").resolves();
    sinon.stub(MedicalRecordMap, "toDTO").callsFake((record) => ({
      id: record.id,
      medicalRecordNumber: record.medicalRecordNumber,
      medicalConditions: record.medicalConditions,
      allergies: record.allergies,
      description: record.description,
    }));

    const service = new MedicalRecordService(medicalRecordRepoInstance, medicalConditionRepoInstance, allergyRepoInstance);
    const result = await service.updateMedicalRecord(medicalRecordDTO2);

    expect(result.isSuccess).to.be.true;
    expect(result.getValue().medicalConditions).to.deep.equal([{ "value": 'FA33.0'}]);
  });


  it("should fetch all medical records successfully", async () => {
    const medicalRecords = [
      {
        id: "1234", medicalRecordNumber: "MR-001", medicalConditions: ["Condition1"], allergies: ["Allergy1"], description: "Description1"
      },
      {
        id: "5678", medicalRecordNumber: "MR-002", medicalConditions: ["Condition2"], allergies: ["Allergy2"], description: "Description2"
      },
    ];

    const medicalRecordDTOs: IMedicalRecordDTO[] = medicalRecords.map((record) => ({
      id: record.id,
      medicalRecordNumber: record.medicalRecordNumber,
      medicalConditions: record.medicalConditions,
      allergies: record.allergies,
      description: record.description,
    }));

    const medicalRecordRepoInstance = Container.get("MedicalRecordRepo") as IMedicalRecordRepo;
    const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
    const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;

    sinon.stub(medicalRecordRepoInstance, "findAll").resolves(medicalRecords);

    sinon.stub(MedicalRecordMap, "toDTO").callsFake((record) => ({
      id: record.id,
      medicalRecordNumber: record.medicalRecordNumber,
      medicalConditions: record.medicalConditions,
      allergies: record.allergies,
      description: record.description,
    }));

    const service = new MedicalRecordService(medicalRecordRepoInstance as IMedicalRecordRepo, medicalConditionRepoInstance as IMedicalConditionRepo, allergyRepoInstance as IAllergyRepo);

    const result = await service.getAllMedicalRecords();

    expect(result.isSuccess).to.be.true;
    expect(result.getValue()).to.deep.equal(medicalRecordDTOs);
  });

  it("should return failure if no medical records are found", async () => {
    const medicalRecordRepoInstance = Container.get("MedicalRecordRepo") as IMedicalRecordRepo;
    const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
    const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;

    sinon.stub(medicalRecordRepoInstance, "findAll").resolves([]);

    const service = new MedicalRecordService(medicalRecordRepoInstance as IMedicalRecordRepo, medicalConditionRepoInstance as IMedicalConditionRepo, allergyRepoInstance as IAllergyRepo);

    const result = await service.getAllMedicalRecords();

    expect(result.isFailure).to.be.true;
    expect(result.errorValue()).to.equal("Medical Records not found");
  });

  it("should throw an error if fetching all medical records fails", async () => {
    const medicalRecordRepoInstance = Container.get("MedicalRecordRepo") as IMedicalRecordRepo;
    const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
    const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;

    sinon.stub(medicalRecordRepoInstance, "findAll").rejects(new Error("Database error"));

    const service = new MedicalRecordService(medicalRecordRepoInstance as IMedicalRecordRepo, medicalConditionRepoInstance as IMedicalConditionRepo, allergyRepoInstance as IAllergyRepo);

    try {
      await service.getAllMedicalRecords();
      throw new Error("Expected method to throw, but it didn't.");
    } catch (error) {
      expect(error.message).to.equal("Database error");
    }
  });


  it("should fetch medical records by filters successfully", async () => {
    const filters: IMedicalRecordQueryFilterParameters = {
        filters: [
            { allergyDesignation: "Peanuts", medicalConditionDesignation: "Asthma" },
        ],
    };

    const medicalRecordDTO: IMedicalRecordDTO = {
      id: "202501000001",
      medicalRecordNumber: "202501000001",
      medicalConditions: ['FA14.0'],
      allergies: ['FA33.0'],
      description: "Patient has a mild condition.",
    };

    const medicalRecordRepoInstance = Container.get("MedicalRecordRepo") as IMedicalRecordRepo;
    const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
    const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;

    const existingRecord = MedicalRecord.create(medicalRecordDTO).getValue();
    sinon.stub(medicalRecordRepoInstance, "findAllByParameters").resolves([existingRecord]);

    const service = new MedicalRecordService(medicalRecordRepoInstance, medicalConditionRepoInstance, allergyRepoInstance);
    const result = await service.getMedicalRecordsByFilters(filters);

    expect(result.isSuccess).to.be.true;
    expect(result.getValue()).to.have.lengthOf.above(0);
  });

    it("should export a medical record successfully", async () => {
        const medicalRecordRepoInstance = Container.get("MedicalRecordRepo") as IMedicalRecordRepo;
        const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
        const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;

        const mockMedicalRecord = {
            id: "1234",
            medicalRecordNumber: "MR-001",
            medicalConditions: ['FB70.0'],
            allergies: ['BZ02.2'],
            description: "Patient has no significant history.",
        };

        sinon.stub(medicalRecordRepoInstance, "findByDomainId").resolves(mockMedicalRecord);

        const service = new MedicalRecordService(
            medicalRecordRepoInstance,
            medicalConditionRepoInstance,
            allergyRepoInstance
        );

        const tempDir = tmp.dirSync({ unsafeCleanup: true });
        const exportInfo = {
            medicalRecordNumber: "MR-001",
            filepath: tempDir.name,
            pass: "1234",
        };

        const result = await service.exportMedicalRecord(exportInfo);

        expect(result.isSuccess).to.be.true;
    });


    it("should throw an error if the medical record is not found", async () => {
        const medicalRecordRepo = Container.get("MedicalRecordRepo") as IMedicalRecordRepo;
        const medicalConditionRepo = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
        const allergyRepo = Container.get("AllergyRepo") as IAllergyRepo;

        sinon.stub(medicalRecordRepo, "findByDomainId").resolves(null);

        const service = new MedicalRecordService(
            medicalRecordRepo,
            medicalConditionRepo,
            allergyRepo
        );

        try {
            await service.exportMedicalRecord({
                medicalRecordNumber: "MR-999",
                filepath: "/fake/path",
                pass: "password",
            });
            throw new Error("Expected method to throw, but it didn't.");
        } catch (error) {
            expect(error.message).to.equal("Medical Record wasn't found with this Medical Record Number.");
        }
    });

    it("should handle errors during PDF generation", async () => {
        const medicalRecordRepo = Container.get("MedicalRecordRepo") as IMedicalRecordRepo;
        const medicalConditionRepo = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
        const allergyRepo = Container.get("AllergyRepo") as IAllergyRepo;

        sinon.stub(medicalRecordRepo, "findByDomainId").throws(new Error("PDF generation failed"));

        const service = new MedicalRecordService(
            medicalRecordRepo,
            medicalConditionRepo,
            allergyRepo
        );

        try {
            await service.exportMedicalRecord({
                medicalRecordNumber: "MR-001",
                filepath: "/fake/path",
                pass: "password",
            });
            throw new Error("Expected method to throw, but it didn't.");
        } catch (error) {
            expect(error.message).to.equal("PDF generation failed");
        }
    });
});
