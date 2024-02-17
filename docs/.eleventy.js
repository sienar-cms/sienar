const syntaxHighlight = require("@11ty/eleventy-plugin-syntaxhighlight");

module.exports = function (eleventyConfig) {
	eleventyConfig.addPlugin(syntaxHighlight, {
		templateFormats: ["md"]
	});
	eleventyConfig.setBrowserSyncConfig({
		server: {
			baseDir: "build",
			index: "index.html"
		}
	});
	eleventyConfig.setLibrary("md", md);

	// File copying
	eleventyConfig.addPassthroughCopy("docs.css");

	// Collections
	eleventyConfig.addCollection("introductionSorted", sortByPageNumber("introduction"));
	eleventyConfig.addCollection("guidesSorted", sortByPageNumber("guides"));
	eleventyConfig.addCollection("pluginsSorted", sortByTitle("plugins"));
	eleventyConfig.addCollection("apiSorted", sortByTitle("api"));

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
	// eleventyConfig.addHandlebarsHelper("findPreviousCourse", function (courses, category, url) {
	// 	const i = findCourseIndex(courses, category, url);
	// 	return courses[category][i - 1];
	// });
	// eleventyConfig.addHandlebarsHelper("findNextCourse", function (courses, category, url) {
	// 	const i = findCourseIndex(courses, category, url);
	// 	return courses[category][i + 1];
	// });
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