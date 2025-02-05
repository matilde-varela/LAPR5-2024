import "reflect-metadata";
import { describe, it, beforeEach, afterEach } from "mocha";
import sinon from "sinon";
import { Container } from "typedi";
import { Result } from "../../../src/core/logic/Result";
import MedicalRecordController from "../../../src/controllers/medicalRecordController";
import IMedicalRecordService from "../../../src/services/IServices/IMedicalRecordService";
import { IMedicalRecordDTO } from "../../../src/dto/IMedicalRecordDTO";
import { Request, Response, NextFunction } from "express";

describe("MedicalRecordController Unit Tests", function () {
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

    const mockMedicalRecordRepo = {
      getAll: sandbox.stub().resolves([]),
    };
    Container.set("MedicalRecordRepo", mockMedicalRecordRepo);

    let medicalRecordServiceClass = require("../../../src/services/medicalRecordService").default;
    let medicalRecordServiceInstance = Container.get(medicalRecordServiceClass);
    Container.set("MedicalRecordService", medicalRecordServiceInstance);
  });

  afterEach(() => {
    sandbox.restore();
    sinon.restore();
  });

  it("should return all medical records successfully", async () => {
    const mockRecords: IMedicalRecordDTO[] = [
      {
        id: "1234", medicalRecordNumber: "MR-001", medicalConditions: ["Condition1"], allergies: ["Allergy1"], description: "Description1",
      },
    ];

    const req: Partial<Request> = {};
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      json: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();

    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "getAllMedicalRecords").resolves(Result.ok(mockRecords));

    const controller = new MedicalRecordController(service);

    // Act
    await controller.getAllMedicalRecords(req as Request, res as Response, next as NextFunction);

    // Assert
    sinon.assert.calledWith(res.json as sinon.SinonStub, mockRecords);
  });

  it("should return 402 if no medical records are found", async () => {
    const req: Partial<Request> = {};
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      send: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();

    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "getAllMedicalRecords").resolves(Result.fail("No records found"));

    const controller = new MedicalRecordController(service);

    // Act
    await controller.getAllMedicalRecords(req as Request, res as Response, next as NextFunction);

    // Assert
    sinon.assert.calledWith(res.status as sinon.SinonStub, 402);
    sinon.assert.calledOnce(res.send as sinon.SinonStub);
  });

  it("should handle unexpected errors in getAllMedicalRecords", async () => {
    const req: Partial<Request> = {};
    const res: Partial<Response> = {};
    const next: Partial<NextFunction> = sinon.stub();

    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "getAllMedicalRecords").throws(new Error("Unexpected error"));

    const controller = new MedicalRecordController(service);

    // Act
    await controller.getAllMedicalRecords(req as Request, res as Response, next as NextFunction);

    // Assert
    sinon.assert.calledOnce(next as sinon.SinonStub);
  });

  it("should create a medical record successfully", async () => {
    const medicalRecordDTO: IMedicalRecordDTO = {
      id: "202412000001",
      medicalRecordNumber: "202412000001",
      medicalConditions: ['FA14.0'],
      allergies: ['FA33.0'],
      description: "Patient has a mild condition.",
    };

    const req: Partial<Request> = { body: medicalRecordDTO };
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      json: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();

    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "createMedicalRecord").resolves(Result.ok(medicalRecordDTO));

    const controller = new MedicalRecordController(service);

    // Act
    await controller.createMedicalRecord(req as Request, res as Response, next as NextFunction);

    // Assert
    sinon.assert.calledWith(res.json as sinon.SinonStub, medicalRecordDTO);
    sinon.assert.calledWith(res.status as sinon.SinonStub, 201);
  });

  it("should return 402 if creating a medical record fails", async () => {
    const medicalRecordDTO: IMedicalRecordDTO = {
      id: "202412000001",
      medicalRecordNumber: "202412000001",
      medicalConditions: ['FA14.0'],
      allergies: ['FA33.0'],
      description: "Patient has a mild condition.",
    };

    const req: Partial<Request> = { body: medicalRecordDTO };
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      send: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();

    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "createMedicalRecord").resolves(Result.fail("Failed to create"));

    const controller = new MedicalRecordController(service);

    // Act
    await controller.createMedicalRecord(req as Request, res as Response, next as NextFunction);

    // Assert
    sinon.assert.calledWith(res.status as sinon.SinonStub, 402);
    sinon.assert.calledOnce(res.send as sinon.SinonStub);
  });

  it("should update a medical record successfully", async () => {
    const medicalRecordDTO: IMedicalRecordDTO = {
      id: "202412000001",
      medicalRecordNumber: "202412000001",
      medicalConditions: ['FA14.0'],
      allergies: ['FA33.0'],
      description: "Patient has a mild condition.",
    };

    const req: Partial<Request> = { body: medicalRecordDTO };
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      json: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();

    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "updateMedicalRecord").resolves(Result.ok(medicalRecordDTO));

    const controller = new MedicalRecordController(service);

    // Act
    await controller.updateMedicalRecord(req as Request, res as Response, next as NextFunction);

    // Assert
    sinon.assert.calledWith(res.json as sinon.SinonStub, medicalRecordDTO);
    sinon.assert.calledWith(res.status as sinon.SinonStub, 201);
  });

  it("should return 404 if updating a medical record fails", async () => {
    const medicalRecordDTO: IMedicalRecordDTO = {
      id: "202412000001",
      medicalRecordNumber: "202412000001",
      medicalConditions: ['FA14.0'],
      allergies: ['FA33.0'],
      description: "Patient has a mild condition.",
    };

    const req: Partial<Request> = { body: medicalRecordDTO };
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      send: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();

    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "updateMedicalRecord").resolves(Result.fail("Failed to update"));

    const controller = new MedicalRecordController(service);

    // Act
    await controller.updateMedicalRecord(req as Request, res as Response, next as NextFunction);

    // Assert
    sinon.assert.calledWith(res.status as sinon.SinonStub, 404);
    sinon.assert.calledOnce(res.send as sinon.SinonStub);
  });

  it("should export medical records successfully", async () => {
    const mockExportInfo = {
      medicalRecordNumber: "202412000001",
      filepath: "/some/path",
      pass: "password",
    };

    const req: Partial<Request> = { body: mockExportInfo };
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      json: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();

    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "exportMedicalRecord").resolves(Result.ok("/some/path/Medical_Record.zip"));

    const controller = new MedicalRecordController(service);

    // Act
    await controller.exportMedicalRecords(req as Request, res as Response, next as NextFunction);

    // Assert
    sinon.assert.calledWith(res.json as sinon.SinonStub, "/some/path/Medical_Record.zip");
    sinon.assert.calledWith(res.status as sinon.SinonStub, 201);
  });

  it("should return 404 if exporting medical records fails", async () => {
    const mockExportInfo = {
      medicalRecordNumber: "202412000001",
      filepath: "/some/path",
      pass: "password",
    };

    const req: Partial<Request> = { body: mockExportInfo };
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      send: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();

    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "exportMedicalRecord").resolves(Result.fail("Failed to export"));

    const controller = new MedicalRecordController(service);

    // Act
    await controller.exportMedicalRecords(req as Request, res as Response, next as NextFunction);

    // Assert
    sinon.assert.calledWith(res.status as sinon.SinonStub, 404);
    sinon.assert.calledOnce(res.send as sinon.SinonStub);
  });

  it("should return 201 and the file path on successful export", async () => {
    const req: Partial<Request> = {
      body: {
        medicalRecordNumber: "MR-001",
        password: "password123",
      },
    };
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      json: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();
  
    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "exportMedicalRecord").resolves(Result.ok("/path/to/exported/file.pdf"));
  
    const controller = new MedicalRecordController(service);
  
    // Act
    await controller.exportMedicalRecords(req as Request, res as Response, next as NextFunction);
  
    // Assert
    sinon.assert.calledWith(res.status as sinon.SinonStub, 201);
    sinon.assert.calledWith(res.json as sinon.SinonStub, "/path/to/exported/file.pdf");
  });
  
  it("should return 404 if export fails", async () => {
    const req: Partial<Request> = {
      body: {
        medicalRecordNumber: "MR-001",
        password: "password123",
      },
    };
    const res: Partial<Response> = {
      status: sinon.stub().returnsThis(),
      send: sinon.stub(),
    };
    const next: Partial<NextFunction> = sinon.stub();
  
    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "exportMedicalRecord").resolves(Result.fail("Export failed"));
  
    const controller = new MedicalRecordController(service);
  
    // Act
    await controller.exportMedicalRecords(req as Request, res as Response, next as NextFunction);
  
    // Assert
    sinon.assert.calledWith(res.status as sinon.SinonStub, 404);
    sinon.assert.calledOnce(res.send as sinon.SinonStub);
  });
  
  it("should call next with an error if an exception occurs", async () => {
    const req: Partial<Request> = {
      body: {
        medicalRecordNumber: "MR-001",
        password: "password123",
      },
    };
    const res: Partial<Response> = {};
    const next: Partial<NextFunction> = sinon.stub();
  
    const service = Container.get<IMedicalRecordService>("MedicalRecordService") as IMedicalRecordService;
    sinon.stub(service, "exportMedicalRecord").throws(new Error("Unexpected error"));
  
    const controller = new MedicalRecordController(service);
  
    // Act
    await controller.exportMedicalRecords(req as Request, res as Response, next as NextFunction);
  
    // Assert
    sinon.assert.calledOnce(next as sinon.SinonStub);
  });
  
});
