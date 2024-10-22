// mdrender.js
function renderMarkdown(markdownText) {
	const htmlContent = marked.parse(markdownText);
	// Create a temporary element to parse and highlight the code blocks
	const tempDiv = document.createElement('div');
	tempDiv.innerHTML = htmlContent;
	// Highlight all code blocks
	tempDiv.querySelectorAll('pre code').forEach((block) => {
		hljs.highlightBlock(block);
	});
	return tempDiv.innerHTML;
}

