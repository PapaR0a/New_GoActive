public interface HAGOUIIComponent
{
	void Init(object data, bool isEditMode);
	object ExportView(int id);
	string GetFormType();
	string GetJsonValue();
	string GetKeyForm();
	void ActiveError();
	void ResetError();
	void SetValue(string value);
}