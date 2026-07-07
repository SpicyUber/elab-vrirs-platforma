import { useState } from 'react'
import Login from '../pages/Login'
import RecentSubmissions from '../pages/RecentSubmissions'
import './App.css'
import { Navigate, Route, Routes } from 'react-router-dom'
import { UserProvider } from '../providers/UserProvider'
import MyReviews from '../pages/MyReviews'
import Course from '../pages/Course'
import Assignment from '../pages/Assignment'
import AssignmentEdit from '../pages/AssignmentEdit'
import MyCourses from '../pages/MyCourses'
import CourseEdit from '../pages/CourseEdit'
import Submission from '../pages/Submission'
import SubmissionEdit from '../pages/SubmissionEdit'
import Users from '../pages/Users'
import UserEdit from '../pages/UserEdit'
import Profile from '../pages/Profile'
import CourseMembers from '../pages/CourseMembers'

function App() {
  const [count, setCount] = useState(0)

  return (
    <UserProvider>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path = "/login" element = {<Login/>}/>
        <Route path = "/recent-submissions" element = {<RecentSubmissions/>}/>
        <Route path = "/grades" element = {<MyReviews/>}/>
        <Route path = "/courses/:courseId" element = {<Course/>}/>
        <Route path = "/courses/:courseId/members" element = {<CourseMembers/>}/>
        <Route path = "/assignments/:assignmentId" element = {<Assignment/>}/>
        <Route path = "/assignments/:assignmentId/edit" element = {<AssignmentEdit/>}/>
        <Route path = "/my-courses" element ={<MyCourses/>}/>
        <Route path = "/courses/:courseId/edit" element ={<CourseEdit/>}/>
        <Route path = "/submissions/:submissionId" element = {<Submission/>}/>
        <Route path = "/submissions/:submissionId/edit" element = {<SubmissionEdit/>}/>
        <Route path = "/users" element = {<Users/>}/>
        <Route path = "/profile" element = {<Profile/>}/>
        <Route 
  path="/users/:userId/edit" 
  element={<UserEdit />} 
/>
        </Routes>
        </UserProvider>
  )
}

export default App
