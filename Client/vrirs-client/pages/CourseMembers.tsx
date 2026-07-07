import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import {
    Avatar,
    Box,
    Button,
    CircularProgress,
    Dialog,
    DialogContent,
    DialogTitle,
    FormControlLabel,
    IconButton,
    Input,
    List,
    ListItem,
    ListItemAvatar,
    ListItemText,
    Stack,
    Switch,
    Tab,
    Tabs,
    TextField,
    Typography,
} from "@mui/material";

import AddIcon from "@mui/icons-material/Add";
import { useServer } from "../hooks/useServer";
import type { UserCourseEnrollmentInfo } from "../types/UserCourseEnrollmentInfo"
import { useUser } from "../hooks/useUser";
import type { UserSearchResultPage } from "../types/UserSearchResultPage";








export default function CourseMembers() {

    const { courseId } = useParams();
    const {user} = useUser();
    const [members, setMembers] = useState<UserCourseEnrollmentInfo[]>([]);
    const [searchResults, setSearchResults] = useState<UserSearchResultPage>();
    const [error, setError] = useState<string>("");

    const [open, setOpen] = useState(false);
    const [tab, setTab] = useState(0);

    const [loading, setLoading] = useState(false);

    const [fullName, setFullName] = useState("");
    const [index, setIndex] = useState("");
    const [email, setEmail] = useState("");

    const [pageNumber, setPageNumber] = useState(1);
    const [entriesPerPage, setEntriesPerPage] = useState(10);

    const {baseUrl} = useServer();

    const [csvFile, setCsvFile] = useState<File | null>(null);

    const [isTeacher, setIsTeacher] = useState(false);

    useEffect(() => {
        loadMembers();
    }, []);


    async function loadMembers() {

        setLoading(true);

        const response = await fetch(
            `${baseUrl}/api/courses/${courseId}/enrolled-users`
        );

        if(!response.ok)
        {
            setLoading(false);
            setError(await response.text());
            return;
        }

        const data = await response.json() as UserCourseEnrollmentInfo[];

        console.log(data)


        setMembers(data);

        setLoading(false);
    }


    async function searchUsers() {

        const params = new URLSearchParams();

      if (fullName) params.append("FullName", fullName);
      if (email) params.append("Email", email);
      if (index) params.append("Index", index);

      params.append("PageNumber", pageNumber.toString());
      params.append("EntriesPerPage", entriesPerPage.toString());

      const response = await fetch(
        `${baseUrl}/api/users/search?${params.toString()}`,
        {
          headers: {
            Authorization: `Bearer ${user?.token}`,
          },
        }
      );

        if(!response.ok)
        {
            setError("Search error!"+await response.text())

        setOpen(false);
return;
        }

        const data = await response.json();

        setSearchResults(
            data.entries ?? data.items ?? data
        );
    }


    async function addMember(userId: string, isTeacher: boolean) {

        const response = await fetch(
            `${baseUrl}/api/courses/${courseId}/enrolled-users/by-id`,
            {
                method: "POST",
                headers: {
                     Authorization: `Bearer ${user?.token}`,
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    UserId: userId,
                    IsTeacher: isTeacher,
                }),
            }
        );


        if (!response.ok){
            return;
            }

        const added =
            await response.json();


        setMembers(prev => [
            ...prev,
            added
        ]);
    }


    async function uploadCsv() {

        if (!csvFile)
            return;


        const formData = new FormData();

        formData.append(
            "emailCsv",
            csvFile
        );


        const response = await fetch(
            `${baseUrl}/api/courses/${courseId}/enrolled-users/by-csv`,
            {
                headers: {
            Authorization: `Bearer ${user?.token}`,
          },
                method: "POST",
                body: formData,
            }
        );


        if (!response.ok)
            {
                alert("Enrollment failed! "+await response.text() + " Check for typos or duplicate enrollments!")
                return;
            }


        const added =
            await response.json();


        setMembers(prev => [
            ...prev,
            ...added
        ]);
    }


    return (
        <Box>

            <Box>
                <Typography variant="h5">
                    Course Members
                </Typography>
                

                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={() => setOpen(true)}
                >
                    Add Member
                </Button>

            </Box>



            {
                loading
                ?
                <CircularProgress />
                :
                <List>

                    {
                        members.map(member => (

                            <ListItem key={member.userId}>

                                <ListItemAvatar>
                                    <Avatar
                                        src={
                                            member.userAvatarInBase64
                                            ?
                                            `data:image/png;base64,${member.userAvatarInBase64}`
                                            :
                                            undefined
                                        }
                                    />
                                </ListItemAvatar>


                                <Typography> 
                                    {member.userFullName} | {member.userEmail}
                                </Typography>

                            </ListItem>

                        ))
                    }

                </List>
            }




            <Dialog
                open={open}
                onClose={() => setOpen(false)}
                fullWidth
            >

                <DialogTitle>
                    Add Members
                </DialogTitle>


                <DialogContent>

                    <Tabs
                        value={tab}
                        onChange={(_, value) => setTab(value)}
                    >
                        <Tab label="Search"/>
                        <Tab label="CSV"/>
                    </Tabs>



                    {
                        tab === 0 &&
                        <Box>

                            <TextField
                                fullWidth
                                label="Full Name"
                                value={fullName}
                                onChange={
                                    e => setFullName(e.target.value)
                                }
                                sx={{ mb: 2 }}
                            />


                            <TextField
                                fullWidth
                                label="Index"
                                value={index}
                                onChange={
                                    e => setIndex(e.target.value)
                                }
                                sx={{ mb: 2 }}
                            />


                            <TextField
                                fullWidth
                                label="Email"
                                value={email}
                                onChange={
                                    e => setEmail(e.target.value)
                                }
                                sx={{ mb: 2 }}
                            />


                            <Button
                                variant="contained"
                                onClick={searchUsers}
                            >
                                Search
                            </Button>



                            <List>
<FormControlLabel
    control={
        <Switch
            checked={isTeacher}
            onChange={(e) => setIsTeacher(e.target.checked)}
        />
    }
    label={isTeacher ? "Add As Teacher" : "Add As Student"}
/>
                                {
                                    searchResults?.userProfiles.map(user => (

                                        <ListItem
                                            key={user.id}
                                            secondaryAction={
                                                <IconButton
                                                    onClick={() =>
                                                        addMember(user.id, isTeacher)
                                                    }
                                                >
                                                    <AddIcon />
                                                </IconButton>
                                            }
                                        >
                                            

                                            <ListItemText
                                                primary={user.fullName}
                                                secondary={user.email}
                                            />

                                        </ListItem>

                                    ))
                                }

                            </List>

                        </Box>
                    }



                    {
                        tab === 1 && <>
                        <Box>

                            <Input
                                type="file"
                                inputProps={{
                                    accept: ".csv"
                                }}
                                onChange={(e) => {

                                    const file =
                                        (e.target as HTMLInputElement)
                                        .files?.[0];

                                    if (file)
                                        setCsvFile(file);
                                }}
                            />


                            <Button
                                sx={{ mt: 2 }}
                                variant="contained"
                                onClick={uploadCsv}
                            >
                                Upload
                            </Button>
                                  </Box>
                                  <Stack>
                                    <Typography variant={"caption"}>{"Email,IsTeacher"}</Typography>
                                    <Typography variant={"caption"}>{"student1@example.com,false"}</Typography> </Stack>       </>             
                    }


                </DialogContent>

            </Dialog>

<Typography variant="caption" color="error">{error}</Typography>
        </Box>
    );
}