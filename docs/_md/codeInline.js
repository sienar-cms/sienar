const {md, defaultRenderer} = require("./util");

const defaultRender = md.renderer.rules.code_inline || defaultRenderer;

module.exports = function (tokens, idx, options, env, self) {
	// Get the token
	const token  = tokens[idx];
	token.attrPush(["class", "language-markup"]);
	return defaultRender(tokens, idx, options, env, self);
}