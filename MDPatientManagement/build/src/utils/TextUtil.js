"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.TextUtil = void 0;
class TextUtil {
    static isUUID(text) {
        return new RegExp('\b[0-9a-f]{8}\b-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-\b[0-9a-f]{12}\b').test(text);
    }
    static isICD11Code(code) {
        return new RegExp('^([A-Z]{2})[0-9]{2}(\.[0-9A-Z])?$').test(code);
    }
}
exports.TextUtil = TextUtil;
//# sourceMappingURL=TextUtil.js.map