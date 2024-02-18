module.exports = function (tokens, idx) {
	// Get the token
	const token  = tokens[idx];
	const srcIndex = token.attrIndex("src");
	const src = token.attrs[srcIndex][1];
	const description = token.children[0].content;

	// Return the render
	return `
		<figure class="my-5 p-4 bg-light">
			<img class="d-block mx-auto" src="${src}" alt="${description}"/>
			<figcaption class="text-center mt-4 fst-italic small">
				${description}
			</figcaption>
		</figure>`;
}