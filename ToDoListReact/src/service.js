import axios from 'axios';

axios.defaults.baseURL  = "http://localhost:5158"
// הוספת Interceptor לתפיסת שגיאות ב־response ורישום ללוג
axios.interceptors.response.use(
  function (response) {
    return response;
  },
  error => {
    console.error('Request failed:', error);
    return Promise.reject(error);
  }
);

const Tasks={
  getTasks: async () => {
    try {
      const result = await axios.get("/todos");
      // console.log(result.data);
      return result.data;
    } catch (error) {
      console.error('Error getting tasks:', error);
      throw error;
    }
  },

  addTask: async(name)=>{
    console.log('addTask', name)
    try {
      const result = await axios.post("/todos",{name});
      return result.data;
    } catch (error) {
      console.error('Error adding tasks:', error);
      throw error;
    }
  },

  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', {id, isComplete})
    try {
      const result = await axios.put(`/todos/${id}`, { isComplete });
      return result.data;
    } catch (error) {
      console.error('Error setting task completion status:', error);
      throw error;
    }
  },

  deleteTask:async(id)=>{
    console.log('deleteTask')
    try {
      await axios.delete(`/todos/${id}`);
    } catch (error) {
      console.error('Error delete tasks:', error);
      throw error;
    }
  }
};
export default Tasks;
