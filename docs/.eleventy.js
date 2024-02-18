const md = require("./_md");

module.exports = function (eleventyConfig) {
	eleventyConfig.setBrowserSyncConfig({
		server: {
			baseDir: "build",
			index: "index.html"
		}
	});
	eleventyConfig.setLibrary("md", md);

	// File copying
	eleventyConfig.setServerPassthroughCopyBehavior("passthrough");
	eleventyConfig.addPassthroughCopy("assets");
	eleventyConfig.addPassthroughCopy("**/*.jpg");

	// Collections
	eleventyConfig.addCollection("introductionSorted", sortByPageNumber("introduction"));
	eleventyConfig.addCollection("guidesSorted", sortByPageNumber("guides"));
	eleventyConfig.addCollection("pluginsSorted", sortByTitle("plugins"));
	eleventyConfig.addCollection("apiSorted", sortByTitle("api"));
	eleventyConfig.addCollection("plugin-providers-sorted", sortByPageNumber("plugin-providers"));

	// Template helpers
	eleventyConfig.addHandlebarsHelper("eq", function (a, b) {
		return a === b;
	});
	eleventyConfig.addHandlebarsHelper("neq", function (a, b) {
		return a !== b;
	});
	eleventyConfig.addHandlebarsHelper("mainMenuActiveClass", function (linkHref, pageUrl) {
		if (linkHref === "/" && pageUrl === "/"
			|| linkHref !== "/" && pageUrl.startsWith(linkHref)) {
			return "active fw-bold";
		}
		return "";
	});
	eleventyConfig.addHandlebarsHelper("returnValueConditional", function(isTrue, trueClassName, falseClassName) {
		return isTrue ? trueClassName : (falseClassName || "");
	});
	eleventyConfig.addHandlebarsHelper("idify", function(input) {
		return input
			.toLowerCase()
			.replaceAll(" ", "-")
			.replace(/[^\w-]/g, "")
	});
	eleventyConfig.addHandlebarsHelper("encodeURIComponent", function (baseUri, appendedUri) {
		return encodeURIComponent(`${baseUri}${appendedUri}`);
	});
	eleventyConfig.addHandlebarsHelper("articleIsInCategory", function (articles, url) {
		return articles.findIndex(c => c.url === url) > -1;
	});
	eleventyConfig.addHandlebarsHelper("findPreviousArticle", function (articles, url) {
		const i = articles.findIndex(c => c.url === url);
		return i === 0
			? null
		 	: articles[i - 1];
	});
	eleventyConfig.addHandlebarsHelper("findNextArticle", function (articles, url) {
		const i = articles.findIndex(c => c.url === url);
		return i === articles.length - 1
			? null
			: articles[i + 1];
	});
};

function sortByPageNumber(category) {
	return function (collections) {
		return collections
			.getFilteredByTag(category)
			.sort((a, b) => a.data.pageNumber - b.data.pageNumber);
	}
}

function sortByTitle(category) {
	return function (collections) {
		return collections
			.getFilteredByTag(category)
			.sort((a, b) => {
				const titleA = a.data.pageTitle.toLowerCase();
				const titleB = b.data.pageTitle.toLowerCase();
				if (titleA < titleB) return -1;
				if (titleA > titleB) return 1;
				return 0;
			});
	}
}