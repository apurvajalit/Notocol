create proc spTestSourceData
AS
begin
set nocount on
select * 
from Source
inner join  SourceTag on Source.ID = SourceTag.SourceID
inner join Tag on SourceTag.TagsID = tag.ID

End