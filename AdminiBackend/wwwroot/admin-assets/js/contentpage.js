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

const buttons = document.getElementsByClassName("btn-paste");
for (let i = 0; i < buttons.length; i++) {
	buttons[i].addEventListener("click", function () {
		const image = `<p><img src="${buttons[i].dataset.url}" alt="" title=""></p>`;
		simplemde.value(simplemde.value() + '\n' + image);
	});
}
