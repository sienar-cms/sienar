const {md} = require("./util");

md.renderer.rules.image = require("./image");
md.renderer.rules.link_open = require("./linkOpen");
md.renderer.rules.code_inline = require("./codeInline");

module.exports = md;