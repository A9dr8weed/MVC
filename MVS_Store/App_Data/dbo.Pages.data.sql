SET IDENTITY_INSERT [dbo].[Pages] ON
INSERT INTO [dbo].[Pages] ([Id], [Title], [Slug], [Body], [Sorting], [HasSidebar]) VALUES (1, N'Home', N'home', N'qwerty', 100, 0)
INSERT INTO [dbo].[Pages] ([Id], [Title], [Slug], [Body], [Sorting], [HasSidebar]) VALUES (2, N'ADSASD', N'123', N'<h2 style="text-align: center;">
	<span style="background-color:#ffff00;">Second</span></h2>
', 100, 1)
SET IDENTITY_INSERT [dbo].[Pages] OFF
