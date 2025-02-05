import "reflect-metadata";
import { it } from 'mocha';
import sinon from "sinon";
import { Container } from "typedi";
import { Response, Request, NextFunction } from 'express';
import MedicalRecordController from "../../src/controllers/medicalRecordController";
import IMedicalRecordService from "../../src/services/IServices/IMedicalRecordService";
import { IMedicalRecordDTO } from "../../src/dto/IMedicalRecordDTO";
import { MedicalRecordMap } from "../../src/mappers/MedicalRecordMap";
import { Result } from "../../src/core/logic/Result";

describe("MedicalRecord Controller + Service Integration Tests", function () {
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

        let medicalRecordSchemaInstance = require("../../src/persistence/schemas/medicalRecordSchema").default;
        Container.set("medicalRecordSchema", medicalRecordSchemaInstance);

        let medicalRecordRepoClass = require("../../src/repos/medicalRecordRepo").default;
        let medicalRecordRepoInstance = Container.get(medicalRecordRepoClass);
        Container.set("MedicalRecordRepo", medicalRecordRepoInstance);

        let medicalRecordServiceClass = require("../../src/services/medicalRecordService").default;
        let medicalRecordServiceInstance = Container.get(medicalRecordServiceClass);
        Container.set("MedicalRecordService", medicalRecordServiceInstance);
    });

    afterEach(() => {
        sandbox.restore();
        sinon.restore();
    });

    it("should return all medical records successfully", async () => {
        const medicalRecordDTOList: IMedicalRecordDTO[] = [
            {
                id: "1234",
                medicalRecordNumber: "MR-001",
                medicalConditions: ["Condition1"],
                allergies: ["Allergy1"],
                description: "Description1"
            },
            {
                id: "1235",
                medicalRecordNumber: "MR-002",
                medicalConditions: ["Condition2"],
                allergies: ["Allergy2"],
                description: "Description2"
            }
        ];

        const req: Partial<Request> = {};
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            json: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalRecordServiceInstance = Container.get("MedicalRecordService") as IMedicalRecordService;
        sinon.stub(medicalRecordServiceInstance, "getAllMedicalRecords").resolves(Result.ok(medicalRecordDTOList));

        sinon.stub(MedicalRecordMap, "toDTO").callsFake((record) => record);

        const ctrl = new MedicalRecordController(medicalRecordServiceInstance);

        // Act
        await ctrl.getAllMedicalRecords(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, sinon.match(medicalRecordDTOList));
    });

    it("should handle error when no medical records are found", async () => {
        const req: Partial<Request> = {};
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            send: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalRecordServiceInstance = Container.get("MedicalRecordService") as IMedicalRecordService;
        sinon.stub(medicalRecordServiceInstance, "getAllMedicalRecords").resolves(Result.fail("Medical Records not found"));

        const ctrl = new MedicalRecordController(medicalRecordServiceInstance);

        // Act
        await ctrl.getAllMedicalRecords(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 402);
        sinon.assert.calledOnce(res.send);
    });

    it("should create a new medical record successfully", async () => {
        const medicalRecordDTO: IMedicalRecordDTO = {
            id: "1236",
            medicalRecordNumber: "MR-003",
            medicalConditions: ["Condition3"],
            allergies: ["Allergy3"],
            description: "Description3"
        };

        const req: Partial<Request> = { body: medicalRecordDTO };
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            json: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalRecordServiceInstance = Container.get("MedicalRecordService") as IMedicalRecordService;
        sinon.stub(medicalRecordServiceInstance, "createMedicalRecord").resolves(Result.ok(medicalRecordDTO));

        const ctrl = new MedicalRecordController(medicalRecordServiceInstance);

        // Act
        await ctrl.createMedicalRecord(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, sinon.match(medicalRecordDTO));
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 201);
    });

    it("should handle error when creating a medical record", async () => {
        const req: Partial<Request> = { body: { id: "1237" } };
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            send: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalRecordServiceInstance = Container.get("MedicalRecordService") as IMedicalRecordService;
        sinon.stub(medicalRecordServiceInstance, "createMedicalRecord").resolves(Result.fail("Error creating medical record"));

        const ctrl = new MedicalRecordController(medicalRecordServiceInstance);

        // Act
        await ctrl.createMedicalRecord(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 402);
        sinon.assert.calledOnce(res.send);
    });

    it("should update an existing medical record successfully", async () => {
        const medicalRecordDTO: IMedicalRecordDTO = {
            id: "1234",
            medicalRecordNumber: "MR-001",
            medicalConditions: ["Condition1", "Condition2"],
            allergies: ["Allergy1"],
            description: "Updated Description"
        };

        const req: Partial<Request> = { body: medicalRecordDTO };
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            json: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalRecordServiceInstance = Container.get("MedicalRecordService") as IMedicalRecordService;
        sinon.stub(medicalRecordServiceInstance, "updateMedicalRecord").resolves(Result.ok(medicalRecordDTO));

        const ctrl = new MedicalRecordController(medicalRecordServiceInstance);

        // Act
        await ctrl.updateMedicalRecord(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, sinon.match(medicalRecordDTO));
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 201);
    });

    it("should handle error when updating a medical record that doesn't exist", async () => {
        const req: Partial<Request> = { body: { id: "1234" } };
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            send: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalRecordServiceInstance = Container.get("MedicalRecordService") as IMedicalRecordService;
        sinon.stub(medicalRecordServiceInstance, "updateMedicalRecord").resolves(Result.fail("Patient Medical Record not found"));

        const ctrl = new MedicalRecordController(medicalRecordServiceInstance);

        // Act
        await ctrl.updateMedicalRecord(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 404);
        sinon.assert.calledOnce(res.send);
    });

    it("should handle filtered medical record queries", async () => {
        const filters = { filters: [{ allergyDesignation: "Peanut", medicalConditionDesignation: "Asthma" }] };
        const medicalRecordDTOList: IMedicalRecordDTO[] = [
            {
                id: "1234",
                medicalRecordNumber: "MR-001",
                medicalConditions: ["Condition1"],
                allergies: ["Allergy1"],
                description: "Description1"
            }
        ];

        const req: Partial<Request> = { body: filters };
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            json: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalRecordServiceInstance = Container.get("MedicalRecordService") as IMedicalRecordService;
        sinon.stub(medicalRecordServiceInstance, "getMedicalRecordsByFilters").resolves(Result.ok(medicalRecordDTOList));

        const ctrl = new MedicalRecordController(medicalRecordServiceInstance);

        // Act
        await ctrl.getFilteredMedicalRecords(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, sinon.match(medicalRecordDTOList));
    });
});
