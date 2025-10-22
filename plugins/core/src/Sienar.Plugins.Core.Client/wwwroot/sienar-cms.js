async function downloadFileFromStream(filename, contentStreamReference) {
	const arrayBuffer = await contentStreamReference.arrayBuffer();
	const blob = new Blob([arrayBuffer]);
	const url = URL.createObjectURL(blob);
	const anchorElement = document.createElement('a');
	anchorElement.href = url;
	anchorElement.download = filename ?? '';
	anchorElement.click();
	anchorElement.remove();
	URL.revokeObjectURL(url);
}