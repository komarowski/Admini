const simplemde = new SimpleMDE({
	element: document.getElementById("simplemde"),
	toolbar:
		[
			{
				name: "image",
				action: () => {
					document.getElementById("imageModal").style.display = 'block';
				},
				className: "fa fa-picture-o",
				title: "Manage images",
			},
			"|",
			{
				name: "bold",
				action: SimpleMDE.toggleBold,
				className: "fa fa-bold",
				title: "Bold",
			},
			{
				name: "italic",
				action: SimpleMDE.toggleItalic,
				className: "fa fa-italic",
				title: "Italic",
			},
			{
				name: "strikethrough",
				action: SimpleMDE.toggleStrikethrough,
				className: "fa fa-strikethrough",
				title: "Strikethrough",
			},
			"|",
			{
				name: "preview",
				action: SimpleMDE.togglePreview,
				className: "fa fa-eye no-disable",
				title: "Toggle Preview",
			},
			{
				name: "fullscreen",
				action: SimpleMDE.toggleFullScreen,
				className: "fa fa-arrows-alt no-disable no-mobile",
				title: "Toggle Fullscreen",
			},
			"|",
			{
				name: "guide",
				action: () => {
					window.open("https://www.markdownguide.org/basic-syntax/", "_blank");
				},
				className: "fa fa-question-circle",
				title: "Markdown Guide",
			},
		]
});

const generateSlider = (imageArray) => {
	let result = '<div class="w4-slider">';

	imageArray.forEach((image) => {
		result += `\n<div class="w4-slide">
  <img src="${image}" alt="" title="">
  <span></span>
</div>`;
	})

	if (imageArray.length !== 1) {
		result += `\n<button class="w4-button-slider w4-button-slider--prev"> < </button>
<button class="w4-button-slider w4-button-slider--next"> > </button>`;
	}
	result += '\n</div>';
	return result;
}

const btnPaste = document.getElementById("btn-paste");
if (btnPaste) {
	btnPaste.onclick = () => {
		let imageArray = [];
		const checkboxes = document.querySelectorAll(".image-checkbox");
		checkboxes.forEach((element) => {
			if (element.checked) {
				imageArray.push(element.value);
			}
		})

		if (imageArray.length >= 1) {
			const silderHtml = generateSlider(imageArray);
			simplemde.value(simplemde.value() + '\n\n' + silderHtml);
		}
	}
}
