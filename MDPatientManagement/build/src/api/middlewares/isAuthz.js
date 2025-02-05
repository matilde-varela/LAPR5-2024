"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const jsonwebtoken_1 = __importDefault(require("jsonwebtoken"));
const config_1 = __importDefault(require("../../../config"));
/**
 * Middleware to verify if the user's role in the JWT matches one of the allowed roles.
 * @param {string[]} allowedRoles - The roles that are allowed to access the route.
 */
const isAuthz = (allowedRoles) => {
    return (req, res, next) => {
        try {
            // Extract the token from the Authorization header
            const authHeader = req.headers.authorization;
            if (!authHeader || !authHeader.startsWith("Bearer ")) {
                return res.status(401).json({ message: "Authorization token is missing or invalid" });
            }
            const token = authHeader.split(" ")[1];
            // Decode and verify the token
            const decodedToken = jsonwebtoken_1.default.verify(token, config_1.default.jwtSecret);
            // Extract roles from the token
            const userRoles = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            // If no roles are present in the token or the user has no matching roles, deny access
            if (!userRoles || !allowedRoles.some(role => userRoles.includes(role))) {
                return res.status(403).json({ message: "Access denied: insufficient privileges" });
            }
            next();
        }
        catch (error) {
            return res.status(500).json({ message: "An error occurred while checking roles", error });
        }
    };
};
exports.default = isAuthz;
//# sourceMappingURL=isAuthz.js.map